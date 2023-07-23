using AutoMapper;
using Business.Dto;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VirtualWallet.Models
{
    public class CustomAutoMapper : Profile
    {
        public CustomAutoMapper()
        {
            //Users
            //Dto

            CreateMap<GetUserDto, User>();
            CreateMap<CreateUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<User, CreateUserDto>();
            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

            //Transfers
            //DTO

            CreateMap<TransferDepositDto, Transfer>();
            CreateMap<Transfer, TransferDepositDto>();
            CreateMap<TransferWithdrawalDto, Transfer>();
            CreateMap<Transfer, TransferWithdrawalDto>();

            //Accounts
            //DTO

            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountDto>();


        }
    }
}
