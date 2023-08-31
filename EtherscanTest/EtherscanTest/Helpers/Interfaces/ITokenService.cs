using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EtherscanTest.Models;

namespace EtherscanTest.Helpers.Interfaces
{
    public interface ITokenService
    {
        List<TokenDetail> GetTokenList(List<TokenDetail> Tokens);

    }
}
