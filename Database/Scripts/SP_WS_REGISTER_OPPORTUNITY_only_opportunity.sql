USE [IDELCOM_DEV]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_WS_REGISTER_OPPORTUNITY]
(
    @BUSINESS_ID                BIGINT,
    @NEG_STAGES_ID             BIGINT,
    @OPPOR_DESC                VARCHAR(150),
    @CLIENTS_ID                BIGINT,
    @BUSINESS_LINE_ID          BIGINT,
    @WORKER_ID                 BIGINT,
    @PORCENT_PROGRESS_PRO      INT = NULL,
    @CURRENCY_ID               BIGINT,
    @OPPOR_AMOUNT              DECIMAL(12,3) = 0.0,
    @REGISTRATION_DATE         DATE,
    @FINISH_DATE               DATE = NULL,
    @CONSULT_DATE              DATETIME = NULL,
    @QUO_DATE                  DATETIME = NULL,
    @CREATE_USER               BIGINT,
    @FOLLOWUP_ENABLED          INT = NULL,
    @FOLLOWUP_EVERY_DAYS       INT = NULL,

    -- Se conserva por compatibilidad con llamadas actuales.
    -- La decision/registro de licitacion debe manejarse fuera de este SP.
    @REQUIRES_LICITATION       BIT = 0,
    @PROCESS_TYPE              INT = 1,
    @CONTACTS_CRM_ID           BIGINT = NULL,

    @PM_CONDITION_ID           BIGINT = NULL,
    @FLOW_TYPE_ID              BIGINT,

    @TYPE_OPPOR                INT = NULL,
    @PARENT_OPPORTUNITY_ID     BIGINT = NULL,

    -- Compatibilidad: este SP ya no procesa entregables de contrataciones.
    @DELIVERABLES_HIRING dbo.TT_OPPOR_DELIVERABLE READONLY
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @status                  INT = 0,
        @message                 VARCHAR(MAX) = NULL,
        @followup_warning        VARCHAR(250) = NULL,
        @post_warning            VARCHAR(500) = NULL,
        @error_number            INT = NULL,
        @error_line              INT = NULL,
        @error_procedure         SYSNAME = NULL,
        @OPPOR_ID                BIGINT = NULL,
        @OPPOR_NUM               VARCHAR(150) = NULL,
        @EXERCISE                INT,
        @ADDITIONAL_SEQUENCE     INT = NULL,
        @NOW                     DATETIME2(0) = SYSDATETIME(),
        @PARENT_TYPE_OPPOR       INT = NULL;

    ------------------------------------------------------------
    -- Constantes
    ------------------------------------------------------------
    DECLARE
        @NEG_STAGE_APOYO         BIGINT = 1,
        @STATE_GEN_EVALUACION    BIGINT = 1,
        @STATE_OPP_PROSPECTO     BIGINT = 1,
        @STATE_GEN_APOYO         BIGINT = 8,
        @STATE_OPP_SOLICITADO    BIGINT = 13,
        @STATE_PRE_SOLICITADO    BIGINT = 1;

    SET @OPPOR_DESC = NULLIF(LTRIM(RTRIM(@OPPOR_DESC)), '');

    BEGIN TRY
        IF @OPPOR_NUM IS NULL
        BEGIN
            DECLARE @T TABLE (id VARCHAR(150));
            INSERT @T EXEC dbo.SP_WS_GET_NEXT_OPPORTUNITY_CODE @BUSINESS_ID = @BUSINESS_ID;
            SELECT @OPPOR_NUM = id FROM @T;
        END

        ------------------------------------------------------------
        -- 0) Validaciones base
        ------------------------------------------------------------
        IF @BUSINESS_ID IS NULL OR @BUSINESS_ID <= 0 THROW 50000, 'BUSINESS_ID inválido.', 1;
        IF @OPPOR_NUM IS NULL THROW 50001, 'OPPOR_NUM es requerido.', 1;
        IF @OPPOR_DESC IS NULL THROW 50002, 'OPPOR_DESC es requerido.', 1;
        IF @NEG_STAGES_ID IS NULL OR @NEG_STAGES_ID <= 0 THROW 50003, 'NEG_STAGES_ID inválido.', 1;
        IF @CLIENTS_ID IS NULL OR @CLIENTS_ID <= 0 THROW 50004, 'CLIENTS_ID inválido.', 1;
        IF @WORKER_ID IS NULL OR @WORKER_ID <= 0 THROW 50005, 'WORKER_ID inválido.', 1;
        IF @BUSINESS_LINE_ID IS NULL OR @BUSINESS_LINE_ID <= 0 THROW 50006, 'BUSINESS_LINE_ID inválido.', 1;
        IF @CURRENCY_ID IS NULL OR @CURRENCY_ID <= 0 THROW 50007, 'CURRENCY_ID inválido.', 1;

        IF @NEG_STAGES_ID <> @NEG_STAGE_APOYO
        BEGIN
            IF @REGISTRATION_DATE IS NULL THROW 50008, 'REGISTRATION_DATE es requerido.', 1;
            IF @FINISH_DATE IS NULL THROW 50009, 'FINISH_DATE es requerido.', 1;
            IF @FINISH_DATE < @REGISTRATION_DATE THROW 50010, 'FINISH_DATE no puede ser menor que REGISTRATION_DATE.', 1;
        END
        ELSE
        BEGIN
            IF @REGISTRATION_DATE IS NULL SET @REGISTRATION_DATE = CAST(@NOW AS DATE);
        END

        ------------------------------------------------------------
        -- 1) Calculo de followup + estados iniciales
        ------------------------------------------------------------
        DECLARE
            @STATE_OPPORTUNITY_ID BIGINT,
            @STATE_GEN_ID BIGINT,
            @STATE_PRE_SALES BIGINT = NULL,
            @IS_PRE_OPPORTUNITY BIT = NULL,
            @FOLLOWUP_NEXT_AT DATETIME2(0) = NULL,
            @baseDate DATETIME2(0),
            @nextDate DATETIME2(0),
            @finishEnd DATETIME2(0);

        IF @NEG_STAGES_ID <> @NEG_STAGE_APOYO
        BEGIN
            SET @baseDate  = CAST(@REGISTRATION_DATE AS DATETIME2(0));
            SET @finishEnd = DATEADD(SECOND, -1, DATEADD(DAY, 1, CAST(@FINISH_DATE AS DATETIME2(0))));

            SET @nextDate =
                CASE
                    WHEN ISNULL(@FOLLOWUP_ENABLED, 0) <> 1 THEN NULL
                    WHEN ISNULL(@FOLLOWUP_EVERY_DAYS, 0) <= 0 THEN NULL
                    ELSE DATEADD(DAY, @FOLLOWUP_EVERY_DAYS, @baseDate)
                END;

            SET @FOLLOWUP_NEXT_AT =
                CASE
                    WHEN @nextDate IS NULL THEN NULL
                    WHEN @nextDate > @finishEnd THEN NULL
                    ELSE @nextDate
                END;

            IF (@nextDate IS NOT NULL AND @nextDate > @finishEnd)
                SET @followup_warning =
                    'No se programó el seguimiento porque la próxima fecha excede la fecha fin de la oportunidad.';

            SET @STATE_GEN_ID = @STATE_GEN_EVALUACION;
            SET @STATE_OPPORTUNITY_ID = @STATE_OPP_PROSPECTO;
        END
        ELSE
        BEGIN
            SET @STATE_GEN_ID = @STATE_GEN_APOYO;
            SET @STATE_OPPORTUNITY_ID = @STATE_OPP_SOLICITADO;
            SET @STATE_PRE_SALES = @STATE_PRE_SOLICITADO;
            SET @IS_PRE_OPPORTUNITY = 1;

            SET @FOLLOWUP_ENABLED = NULL;
            SET @FOLLOWUP_EVERY_DAYS = NULL;
            SET @FOLLOWUP_NEXT_AT = NULL;
        END

        IF @EXERCISE IS NULL
        BEGIN
            SELECT TOP 1
                @EXERCISE = DESCRIPTION
            FROM EXERCISES
            WHERE DESCRIPTION = YEAR(GETDATE());

            IF @EXERCISE IS NULL
                THROW 50022, 'No existe ejercicio activo para el año actual.', 1;
        END

        ------------------------------------------------------------
        -- 2) Validaciones para oportunidades adicionales
        ------------------------------------------------------------
        IF @TYPE_OPPOR = 2
        BEGIN
            IF @PARENT_OPPORTUNITY_ID IS NULL OR @PARENT_OPPORTUNITY_ID <= 0
                THROW 50030, 'La oportunidad principal es obligatorio si agregar un adicional', 1;

            SELECT TOP 1
                @PARENT_TYPE_OPPOR = TYPE_OPPOR
            FROM OPPORTUNITY
            WHERE OPPOR_ID = @PARENT_OPPORTUNITY_ID
              AND BUSINESS_ID = @BUSINESS_ID;

            IF @PARENT_TYPE_OPPOR IS NULL
                THROW 50031, 'La oportunidad padre no existe o no pertenece a la empresa enviado.', 1;

            IF @PARENT_TYPE_OPPOR <> 1
                THROW 50032, 'La oportunidad padre de un adicional debe ser una oportunidad principal.', 1;
        END
        ELSE
        BEGIN
            SET @PARENT_OPPORTUNITY_ID = NULL;
            SET @ADDITIONAL_SEQUENCE = NULL;
        END

        ------------------------------------------------------------
        -- 3) Transaccion: solo oportunidad
        ------------------------------------------------------------
        BEGIN TRAN;

            EXEC dbo.SP_OPPOR_INSERT_CORE
                @BUSINESS_ID           = @BUSINESS_ID,
                @NEG_STAGES_ID         = @NEG_STAGES_ID,
                @OPPOR_NUM             = @OPPOR_NUM,
                @OPPOR_DESC            = @OPPOR_DESC,
                @CLIENTS_ID            = @CLIENTS_ID,
                @BUSINESS_LINE_ID      = @BUSINESS_LINE_ID,
                @WORKER_ID             = @WORKER_ID,
                @PORCENT_PROGRESS_PRO  = @PORCENT_PROGRESS_PRO,
                @STATE_OPPORTUNITY_ID  = @STATE_OPPORTUNITY_ID,
                @STATE_GEN_ID          = @STATE_GEN_ID,
                @STATE_PRE_SALES_ID    = @STATE_PRE_SALES,
                @IS_PRE_OPPORTUNITY    = @IS_PRE_OPPORTUNITY,
                @CURRENCY_ID           = @CURRENCY_ID,
                @OPPOR_AMOUNT          = @OPPOR_AMOUNT,
                @REGISTRATION_DATE     = @REGISTRATION_DATE,
                @FINISH_DATE           = @FINISH_DATE,
                @CONSULT_DATE          = @CONSULT_DATE,
                @QUO_DATE              = @QUO_DATE,
                @FOLLOWUP_ENABLED      = @FOLLOWUP_ENABLED,
                @FOLLOWUP_EVERY_DAYS   = @FOLLOWUP_EVERY_DAYS,
                @FOLLOWUP_NEXT_AT      = @FOLLOWUP_NEXT_AT,
                @IS_HIRING             = @REQUIRES_LICITATION,
                @CREATE_USER           = @CREATE_USER,
                @CONTACTS_CRM_ID       = @CONTACTS_CRM_ID,
                @PM_CONDITION_ID       = @PM_CONDITION_ID,
                @FLOW_TYPE_ID          = @FLOW_TYPE_ID,
                @EXERCISE              = @EXERCISE,
                @TYPE_OPPOR            = @TYPE_OPPOR,
                @PARENT_OPPORTUNITY_ID = @PARENT_OPPORTUNITY_ID,
                @ADDITIONAL_SEQUENCE   = @ADDITIONAL_SEQUENCE,
                @OPPOR_ID              = @OPPOR_ID OUTPUT;

            DECLARE @CHANGE_REASON_OPP VARCHAR(350) =
                CASE WHEN @NEG_STAGES_ID = @NEG_STAGE_APOYO
                     THEN 'Creación de Apoyo comercial'
                     ELSE 'Creación de prospecto'
                END;

            EXEC dbo.OPPOR_STATE_HISTORY
                @BUSINESS_ID = @BUSINESS_ID,
                @OPPOR_ID = @OPPOR_ID,
                @FROM_STATE_OPPORTUNITY_ID = NULL,
                @TO_STATE_OPPORTUNITY_ID = @STATE_OPPORTUNITY_ID,
                @CHANGE_REASON = @CHANGE_REASON_OPP,
                @CHANGED_BY = @CREATE_USER;

            EXEC dbo.SP_UPDATE_OPPORTUNITY_STATE_CORE_GEN
                @OPPOR_ID = @OPPOR_ID,
                @BUSINESS_ID = @BUSINESS_ID,
                @STATE_GEN_ID = @STATE_GEN_ID,
                @UPDATE_USER = @CREATE_USER,
                @CHANGE_REASON_OPP = 'Creación de estado general EVALUACIÓN',
                @OLD_STATE_GEN_ID = NULL;

        COMMIT TRAN;

        ------------------------------------------------------------
        -- 4) Post-action no critica: alerta de seguimiento
        ------------------------------------------------------------
        IF @FOLLOWUP_NEXT_AT IS NOT NULL
        BEGIN
            BEGIN TRY
                DECLARE
                    @USERS_ID BIGINT = NULL,
                    @LINK_URL VARCHAR(750) = NULL,
                    @alertMsg VARCHAR(350) = NULL,
                    @TITLEM VARCHAR(350) = NULL;

                SET @alertMsg = CONCAT('Tienes un seguimiento programado para la oportunidad ', @OPPOR_NUM, '.');
                SET @TITLEM = CONCAT('Seguimiento de oportunidad ', @OPPOR_NUM, '.');

                SELECT TOP 1 @USERS_ID = U.USERS_ID
                FROM dbo.USERS U
                WHERE U.WORKER_ID = @WORKER_ID
                  AND U.BUSINESS_ID = @BUSINESS_ID
                ORDER BY U.USERS_ID DESC;

                IF @USERS_ID IS NULL THROW 50013, 'No existe USERS para el WORKER_ID enviado.', 1;

                SET @LINK_URL = CONCAT('/crm/opportunity?opporNum=', @OPPOR_NUM);

                EXEC dbo.SP_WS_OPPOR_ALERT
                     @BUSINESS_ID        = @BUSINESS_ID
                    ,@OPPOR_ID           = @OPPOR_ID
                    ,@ALERT_CODE         = 1
                    ,@SEVERITY_ID        = 3
                    ,@ALERT_STATUS_ID    = 1
                    ,@TARGET_USER_ID     = @USERS_ID
                    ,@TITLE              = @TITLEM
                    ,@MESSAGE            = @alertMsg
                    ,@LINK_URL           = @LINK_URL
                    ,@FIRST_TRIGGERED_AT = @FOLLOWUP_NEXT_AT;
            END TRY
            BEGIN CATCH
                SET @post_warning = CONCAT(COALESCE(@post_warning + ' | ', ''), 'Falló alerta: ', ERROR_MESSAGE());
            END CATCH
        END

        SET @status = 1;
        SET @message = 'Se registró la oportunidad correctamente.';

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;

        SET @error_number = ERROR_NUMBER();
        SET @error_line = ERROR_LINE();
        SET @error_procedure = ERROR_PROCEDURE();

        IF @error_number IN (2601, 2627)
            SET @message = 'Ya existe una oportunidad con el mismo OPPOR_NUM para este BUSINESS.';
        ELSE
            SET @message = CONCAT(
                'Error en SP_WS_REGISTER_OPPORTUNITY: ',
                ERROR_MESSAGE(),
                ' (Línea ', @error_line,
                ', Procedimiento: ', COALESCE(@error_procedure, 'N/A'),
                ', Número: ', @error_number,
                ')'
            );

        SET @status = 0;
    END CATCH;

    SELECT
        @message AS message,
        @status AS status,
        @OPPOR_ID AS oppor_id,
        @OPPOR_NUM AS oppor_num,
        @error_number AS error_number,
        @error_line AS error_line,
        @error_procedure AS error_procedure,
        @followup_warning AS followup_warning,
        @post_warning AS post_warning;
END
GO
