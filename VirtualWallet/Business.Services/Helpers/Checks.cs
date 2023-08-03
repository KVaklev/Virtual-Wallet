using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Services.Helpers
{
    public class Checks
    {
        
        private string errorMessage = null;

        //public string async Task<string> ChecksOperatorAsync(List<CheckMethodType> CheckMethods, CheckVariables variables)
        //{
        //    var result = new List<string>();
        //    foreach (var checkMetod in CheckMethods)
        //    {
        //        switch (checkMetod)
        //        {
        //            case CheckMethodType.IsTransactionSenderAsync:
        //                return IsTransactionSenderAsync(transaction, userId);
        //            case CheckMethodType.CanModifyTransactionAsync:
        //                return CanModifyTransactionAsync(transaction, userId);
        //            case CheckMethodType.IsAdminAsync:
        //                return IsAdminAsync(User loggedUser);
        //            default:
        //                return awaiat Constants.ModifyAuthorizedErrorMessage;
        //        }


        //    }
        //}

        private async Task<String> CanModifyTransactionAsync(Transaction transaction)
        {
           
            if (transaction.IsExecuted
                    || transaction.Direction == DirectionType.In
                    || transaction.IsDeleted)
            {
                errorMessage= Constants.ModifyTransactionNotExecuteErrorMessage;
            }
            return errorMessage;
        }

        private async Task<string> IsTransactionSenderAsync(Transaction transaction, int userId)
        {
            if (transaction.AccountSender.User.Id != userId)
            {
               errorMessage = Constants.ModifyAuthorizedErrorMessage;
            }
            return errorMessage;
        }

        private async Task<string> IsAdminAsync(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                errorMessage = Constants.ModifyAuthorizedErrorMessage;
            }
            return errorMessage;
        }

    }
}
