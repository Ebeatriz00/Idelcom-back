using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    internal interface IUseCase
    {
        public interface IUseCase<in TRequest, TResponse>
        {
            Task<TResponse> Handle(TRequest request, CancellationToken ct);
        }
    }
}
