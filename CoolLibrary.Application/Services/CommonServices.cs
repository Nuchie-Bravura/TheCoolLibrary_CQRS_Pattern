using AutoMapper;
using CoolLibrary.Domain.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.Services
{
    public class CommonServices
    {
        private readonly IAuthors _authorsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger  _logger;

        public CommonServices(IAuthors authorsRepository, IMapper mapper, ILogger<CommonServices> logger)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _logger = logger;
        }
    }
}
