﻿using Microsoft.AspNetCore.DataProtection;
using static System.Net.Mime.MediaTypeNames;

namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constant messages for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string UpdateStatusUserErrorMessage = "Only an admin or owner of the account is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";

        public const string NoUsersErrorMessage = "No users are found.";
        public const string NoUsersAfterFilterErrorMessage = "No users match the specified filter criteria.";
        public const string NoUserFoundErrorMessage = "No user is found.";

        public const string UsernameExistsErrorMessage = "User with this username already exists.";
        public const string UsernameDoesntExistErrorMessage = "User with this username doesn't exist.";
        public const string EmailExistsErrorMessage = "User with this email already exists.";
        public const string PhoneNumberExistsErrorMessage = "User with this phone number already exists.";

        public const string SuccessfullDeletedUserMessage = "User was successfully deleted.";
        public const string SuccessfullPromoteddUserMessage = "User was successfully promoted with admin rights.";
        public const string SuccessfullBlockedUserMessage = "User was successfully blocked.";
        public const string SuccessfullUnblockedUserMessage = "User was successfully unblocked.";

        //Constant message for Security menu

        public const string ChangePasswordMessage = "🚀 How to Supercharge Your Password?";
        public const string ChangePasswordMessageFirst = "1. ** Be a Password Picasso ** : Imagine your password as a unique work of art! @#$%^&*()—create a masterpiece only you can decipher!";
        public const string ChangePasswordMessageSecond = "2. ** Stretch it Out ** : Think of your password's length as a digital shield. The longer, the better! Aim for a minimum of 8 characters!";
        public const string ChangePasswordMessageThird = "3. ** Mix & Match, But No Predictable Pairs ** : Play matchmaker with letters, numbers, and symbols. Avoid predictable duos like 'Aa' or '123'. Your password should be as unique as you are!❤️";
        public const string ChangePasswordMessageFourth = "4. ** Lowercase Love ** : Show some affection to lowercase letters! Sneak them into your password!";
        public const string ChangePasswordMessageFifth = "5. ** Embrace the Uppercase ** : Give your password a confidence boost with uppercase letters.";
        public const string ChangePasswordMessageSixth = "6. ** Digits Do the Trick ** : Introduce digits to the party! They add complexity and a touch of mystery to your password, making it a tough nut to crack!";
        public const string ChangePasswordMessageSeventh = "7. ** The Special Ingredient ** : Don't forget to sprinkle in special characters! They make your password unbreakable! Choose from characters like !@#$%^&*() to keep things interesting.";
        public const string ChangePasswordMessageEight = "8. ** Stay Original, Stay Safe ** : Avoid clichés like 'password123'. Password is your ultimate secret weapon!";

        //Constant messages for Terms of Use and Privacy Policy

        public const string TermsOfUseMessageFirst = "🌟 These Terms of Use (\"Terms\") govern your access to and use of this web application (the \"Service\") powered by a team of brilliant minds (\"we,\" \"us,\" or \"our\") who have come together to create a seamless experience for you. By using the Service, you agree to be bound by these Terms.";
        public const string TermsOfUseMessageSecond = "By engaging with the Service, you enter into a partnership with us, entrusting your financial well-being to a team that's dedicated to your success. These Terms encapsulate our commitment to your privacy, security, and satisfaction. Your agreement to these Terms is not only a legal obligation but a pact between you and the brilliant minds behind this application.";
        public const string TermsOfUseMessageThird = "In your hands, you hold more than an app; you hold a gateway to financial empowerment, convenience, and peace of mind. Welcome to our digital ecosystem—your Virtual Wallet—where your financial aspirations take center stage!🌟";
        public const string TermsOfUseMessageFourth = "By accessing or using the Service, you acknowledge that you have read, understood, and agree to be bound by these Terms. If you do not agree to these Terms, please do not use the Service.";
        public const string TermsOfUseMessageFifth = "You agree to use the Service in compliance with all applicable laws, regulations, and these Terms. You may not use the Service for any unlawful or unauthorized purpose.";
        public const string TermsOfUseMessageSixth = "To use certain features of the Service, you may need to create a user account. You are responsible for maintaining the confidentiality of your account credentials.";
        public const string TermsOfUseMessageSeventh = "Your use of the Service is also governed by our Privacy Policy, which can be found on the same page. Please read it carefully!";
        public const string TermsOfUseMessageEight = "When making transactions through the Service, you agree to provide accurate and complete payment information. You are responsible for all charges incurred through your account.";
        public const string TermsOfUseMessageNinth = "The content and materials are protected by intellectual property rights.You may not copy, reproduce, distribute, or create works without our prior written consent.";
        public const string TermsOfUseMessageTenth = "We reserve the right to suspend or terminate your access to the Service at any time for any reason, including violation of these Terms.";

        public const string PrivacyPolicyMessageFirst = "We gather various user data such as personal information (username, email, phone), transaction details, usage patterns, and device specifics for improved services.";
        public const string PrivacyPolicyMessageSecond = "Collected data is used to maintain the Service, process transactions, provide support, enhance user experience, and meet legal obligations.";
        public const string PrivacyPolicyMessageThird = "We may share your details with third-party providers assisting in Service operation, and in compliance with legal requirements.";
        public const string PrivacyPolicyMessageFourth = "Manage your account details, adjust cookie settings, and control marketing communications to tailor your experience.";
        public const string PrivacyPolicyMessageFifth = "We take appropriate measures to safeguard personal information from unauthorized access, alteration, or destruction.";
        public const string PrivacyPolicyMessageSixth = "Our Service is designed for users aged 18 and above; we don't knowingly collect data from those under 18.";
        public const string PrivacyPolicyMessageSeventh = "We may update our Privacy Policy periodically, with any revisions becoming effective immediately upon posting.";
        public const string PrivacyPolicyMessageEight = "We keep your personal information only for the time needed to fulfill the purposes outlined here, and we securely delete or anonymize it when it's no longer required.";
        public const string PrivacyPolicyMessageNinth = "Our Service may include links to external websites or services not under our control. We aren't responsible for their privacy practices, so we recommend reviewing their policies before using those sites.";
        public const string PrivacyPolicyMessageTenth = "You have rights regarding your personal information, including access, correction, data portability, deletion, and withdrawal of consent. Reach out to us for more details on how to exercise these rights.";

        //Constant messages for Home Page

        public const string WhyChooseReasonOne = "Speed Pay offers a user-friendly interface and intuitive features, making it effortless to navigate and manage your finances. Its simple design ensures that even those unfamiliar with online banking can easily handle transactions and monitor their funds.";
        public const string WhyChooseReasonTwo = "With Speed Pay, you can experience the convenience of rapid transactions that ensure your money moves swiftly. Whether you're sending funds to family or settling bills, the platform's efficient processing ensures your payments reach their destination promptly.";
        public const string WhyChooseReasonThree = "Opting for a Speed Pay account means enjoying the advantage of reduced transaction fees compared to traditional banking methods. By minimizing costs associated with transfers and payments, you can make the most of your hard-earned money.";
        public const string WhyChooseReasonFour = "Your financial security is a top priority with Speed Pay. Utilizing advanced encryption and stringent security protocols, the platform ensures that your personal and financial information remains confidential and well-protected, giving you peace of mind while conducting transactions online.";

        //Constant messages for FAQ Page

        public const string FirstQuestionMessage = "If you forgot your password, you typically need to use the <strong>\"Forgot Password\"</strong> option on the login form. This usually involves providing your email address or username associated with your account. The platform will then send you instructions on how to reset your password. Follow those instructions to create a new password.";
        public const string SecondQuestionMessage = "This proccess involves linking a bank account or payment method and specifying the amount you want to withdraw. Follow the on-screen instructions to complete the withdrawal.";
        public const string ThirdQuestionMessage = "To link a bank account to your account, you'll typically navigate to your account settings and look for an option like <strong>\"Link Bank Account\"</strong> or <strong>\"Add Payment Method.\"</strong> You'll likely need to provide your bank account details, such as the account number and routing number. Follow the prompts to verify and link your account.";
        public const string FourthQuestionMessage = "To confirm your email address, check your inbox for an email from the platform. This email might contain a link or instructions to verify your email address. Click the link or follow the instructions to confirm your email.";
        public const string FifthQuestionMessage = "To receive payments, you may need to share your account details with the person or entity making the payment. This could include your email address, username, or a specific payment link generated by the platform. Once the payment is sent, you should receive a notification or confirmation.";
        public const string SixthQuestionMessage = "Look for the <strong>\"History\" </strong>section in your Dashboard menu. Here, you should find a list of all payments received and sent, along with details such as dates and amounts.";
        public const string SeventhQuestionMessage = "If you're waiting for a refund, check the <strong>\"Refunds\"</strong> or <strong>\"Order History\"</strong> section in your account. If you don't see any updates, you might need to contact customer support for the platform or service.";
        public const string EightQuestionMessage = "Depending on the platform, there may be an option to <strong>\"Request Payment\"</strong> or <strong>\"Send Invoice.\"</strong> This often involves specifying the recipient's details, payment amount, and a description of the transaction. Once sent, the recipient will receive instructions on how to make the payment.";
        public const string NinthQuestionMessage = "To close your account, navigate to your account settings and look for an option like or <strong>\"Deactivate Account.\"</strong> Follow the provided steps, which may involve confirming your decision and providing a reason for closing the account.";
        public const string TenthQuestionMessage = "To update your contact information, log in to your account and navigate to the <strong>\"Account\"</strong> menu and then <strong>\"My Profile\"</strong> section. Look for an option to edit your contact details, which may include your email address, phone number, and other relevant information. Make sure to save your changes after updating your contact information.";

        //Constant messages for account

        public const string NoAccountsErrorMessage = "No accounts are found.";
        public const string ModifyAccountErrorMessage = "Only an admin or owner of the account can access, delete or modify the acount.";
        public const string ModifyAccountCardErrorMessage = "Only an admin or owner of the account can add or remove a card.";
        public const string ModifyAccountBalancetErrorMessage = "Insufficient balance.";

        //Constant messages for cards

        public const string NoCardsErrorMessage = "No cards are found.";
        public const string NoCardsByAccountSearchErrorMessage = "Account with this ID does not have any cards.";
        public const string ModifyCardErrorMessage = "Only an admin or owner of the card can access, delete or modify the card.";
        public const string CardNumberAddErrorMessage = "Card with the specified card number has already been registered.";
        public const string SuccessfullDeletedCardMessage = "Card has been successfully deleted.";
        public const string NoCardFoundErrorMessage = "No card is found.";

        //Constant messages for transactions 

        public const string ModifyTransactionConfirmMessage = "The transaction has been successfully executed.";
        public const string ModifyTransactionDeleteMessage = "The transacion has been successfully deleted.";
        public const string ModifyTransactionNotExecuteErrorMessage = "You cannot update or delete a completed transaction!";
        public const string ModifyTransactionBlockedErrorMessage = "You are not allowed to make transactions while being blocked!";
        
        //Constant messages for transfers

        public const string ModifyTransferErrorMessage = "You are not authorized to create, delete or modify the transfer.";
        public const string ModifyTransferNoDataErrorMessage = "There are no results related to these parameters.";
        public const string ModifyTransferGetByIdErrorMessage = "You are not authorized for the specified action.";
        public const string ModifyTransferUpdateDeleteErrorErrorMessage = "The transfer is either processed or cancelled so you cannot update or delete it.";
        public const string ModifyDeleteTransfer = "The transfer has been successfully deleted.";
        public const string ModifyExecutedTransfer = "The transfer has been successfully confirmed.";
        public const string ModifyNoRecordsFound = "No records found";

        //Constant messages for curencies

        public const string ModifyCurrencyDeleteMessage = "The currency has been successfully deleted.";
        public const string CurrencyNotFoundErrorMessage = "This currency does not exist.";

        //Constant messages for Authorized

        public const string ModifyAuthorizedErrorMessage = "You are not authorized for the specified action.";

        //Constant messages for ExchangeRate

        public const string ModifyHttpRequestExceptionErrorMessage = "No such host is known.";
        public const string ModifyJsonExceptionErrorMessage = "JSON Deserialization Error.";

        //Constant messages for token

        public const string GenerateTokenErrorMessage = "An error occurred in generating the token";
        public const string SuccessfullTokenMessage = "Logged in successfully. Token: ";
        public const string SuccessfullLoggedInMessage = "Logged in successfully!";

        //Constant messages for registration 

        public const string ConfirmedRegistrationMessage = "Registration confirmed!";
        public const string NotSuccessfullRegistrationMessage = "Something went wrong. Your registration was not successfull.";
        public const string SuccessfullConfirmationEmailSentMessage = "Hooray! Your confirmation email has been sent successfully. Check your inbox, follow the link to complete your registration, and then dive right in to create your wallet!";
        public const string InvalidSendEmailOperation = "An error occurred while sending the email. Please try again later.";

        //Constant messages for login

        public const string CredentialsErrorMessage = "Username and/or Password not specified";
        public const string FailedLoginAtemptErrorMessage = "Nice try! Invalid credentials!";
        public const string NotConfirmedEmailErrorMessage = "Your email is not confirmed, please check your inbox folder and follow the link!";
        public const string NotLoggedInErrorMessage = "Oops, it looks like you've stumbled upon a hidden treasure! " +
                                                      "The menu you're trying to access is reserved for our lovely authenticated members. " +
                                                      "Navigate to the upper right corner and unveil the magic. Either Login or Register!";

        //Other Constant messages 

        public const string NotFoundResults = "No results found.";
        public const string NoHostError= "Such a host is not known.";
        public const string JsonDeserializationError = "JSON Deserialization Error";
        public const string ArgumentOutOfRangeError = "Index and length must refer to a location within the string.";

        public static class PropertyName
        {
            public const string Username = "Username";
            public const string Email = "Email";
            public const string PhoneNumber = "PhoneNumber";
            public const string CardNumber = "CardNumber";
            public const string Credentials = "Credentials";
            public const string PasswordHashMatch = "PasswordHashMatch";
            public const string NotConfirmedEmail = "NotConfirmedEmail";
            public const string UsernameDoesntExist = "UsernameDoesntExist";
        }
    }
}
