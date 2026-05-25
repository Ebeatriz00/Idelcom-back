-- ============================================================
-- Tipos de movimiento de ajuste negativo
-- MOV_OPER_ID = 2 -> Output / disminuye stock
-- IS_ADJUSTMENT = 1
-- ============================================================

DECLARE @BusinessId BIGINT = 1;
DECLARE @MovSunatId BIGINT = 6;
DECLARE @MovPerId   BIGINT = 0;
DECLARE @MovClasId  BIGINT = 1;
DECLARE @CreateUser BIGINT = 1;

-- Marcar ajustes positivos existentes como ajustes
UPDATE MOVEMENT_TYPES
SET
    IS_ADJUSTMENT = 1,
    UPDATE_USER = @CreateUser,
    UPDATE_DATE = GETDATE()
WHERE BUSINESS_ID = @BusinessId
  AND CODE IN ('009', '010', '011')
  AND IS_ADJUSTMENT = 0;

-- 015 - Ajuste por faltante
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '015'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '015',
        'AJUSTE POR INVENTARIO FÍSICO (FALTANTE)',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 016 - Regularización negativa
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '016'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '016',
        'REGULARIZACIÓN NEGATIVA',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 017 - Merma
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '017'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '017',
        'MERMA',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 018 - Baja por daño
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '018'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '018',
        'BAJA POR DAÑO',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 019 - Baja por vencimiento
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '019'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '019',
        'BAJA POR VENCIMIENTO',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 020 - Robo / pérdida
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '020'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '020',
        'ROBO / PÉRDIDA',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;

-- 021 - Corrección negativa de stock
IF NOT EXISTS (
    SELECT 1
    FROM MOVEMENT_TYPES
    WHERE BUSINESS_ID = @BusinessId
      AND CODE = '021'
)
BEGIN
    INSERT INTO MOVEMENT_TYPES
    (
        BUSINESS_ID,
        CODE,
        DESCRIPTION,
        MOV_CLAS_ID,
        MOV_OPER_ID,
        MOV_PER_ID,
        MOV_SUNAT_ID,
        AFFECTS_STOCK,
        REQUIRES_DEST_WARE,
        GENERATES_ACCOUNTING,
        IS_ADJUSTMENT,
        ALLOW_NEGATIVE,
        CREATE_USER,
        CREATE_DATE,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        '021',
        'CORRECCIÓN NEGATIVA DE STOCK',
        @MovClasId,
        2,
        @MovPerId,
        @MovSunatId,
        1,
        0,
        0,
        1,
        0,
        @CreateUser,
        GETDATE(),
        1
    );
END;