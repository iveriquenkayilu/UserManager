using AutoMapper;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Common.Interfaces.Services;
using RainyCorp.UserManagerService.Interfaces.Repositories;
using RainyCorp.UserManagerService.Shared.Interfaces.Services;
using System;

namespace RainyCorp.UserManagerService.Services
{
    /// <summary>
    /// Implements the base service.
    /// </summary>
    public class BaseService : IBaseService
    {
        private bool _disposed;


        protected virtual IUnitOfWork UnitOfWork { get; }

        protected virtual ILogger<BaseService> Logger { get; }

        protected virtual IMapper Mapper { get; }

        protected virtual IUserContext UserContext { get; }

        public BaseService(IUnitOfWork unitOfWork, ILogger<BaseService> logger, IMapper mapper, IUserContext userContext)
        {
            UnitOfWork = unitOfWork;
            Logger = logger;
            Mapper = mapper;
            UserContext = userContext;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _disposed = true;
                UnitOfWork.Dispose();
            }
        }
    }
}