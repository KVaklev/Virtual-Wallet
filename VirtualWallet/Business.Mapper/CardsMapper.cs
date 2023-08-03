using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class CardsMapper : Profile
    {
        public CardsMapper()
        {
            //DTO
            CreateMap<GetCardDto, Card>()
               .ForPath(c => c.Account.User.Username, c => c.MapFrom(c => c.Username))
               .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
               .ReverseMap();

            CreateMap<CreateCardDto, Card>()
                .ForPath(c => c.Account.User.Username, c => c.MapFrom(c => c.AccountUsername))
                .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
                .ForPath(c => c.Currency.CurrencyCode, c =>c.MapFrom(c =>c.CurrencyCode))
                .ReverseMap();

            CreateMap<UpdateCardDto, Card>()
               .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
               .ForPath(c => c.Currency.CurrencyCode, c => c.MapFrom(c => c.CurrencyCode))
               .ReverseMap();

            CreateMap<Card, UpdateCardDto>();

            CreateMap<GetUpdatedCardDto, Card>();
            CreateMap<Card, GetUpdatedCardDto>();
        }

        public static Task<Card> MapCreateDtoToCardAsync(int accountId, Card cardToCreate, Currency currency, Card card)
        {
            cardToCreate.AccountId = accountId;
            cardToCreate.CardNumber = card.CardNumber;
            cardToCreate.CardType = card.CardType;
            cardToCreate.Balance = card.Balance;
            cardToCreate.CardHolder = card.CardHolder;
            cardToCreate.ExpirationDate = card.ExpirationDate;
            cardToCreate.CheckNumber = card.CheckNumber;
            cardToCreate.CreditLimit = card.CreditLimit;
            cardToCreate.CurrencyId = currency.Id;

            return Task.FromResult(cardToCreate);
        }

        public async static Task<Card> MapUpdateDtoToCardAsync(Card cardToUpdate, UpdateCardDto updateCardDto, Currency currency)
        {
            cardToUpdate.CardNumber = updateCardDto.CardNumber ?? cardToUpdate.CardNumber;
            cardToUpdate.CheckNumber = updateCardDto.CheckNumber ?? cardToUpdate.CheckNumber;
            cardToUpdate.CardHolder = updateCardDto.CardHolder ?? cardToUpdate.CardHolder;
            cardToUpdate.CreditLimit = updateCardDto.CreditLimit ?? cardToUpdate.CreditLimit;

            await UpdateCardTypeAsync(updateCardDto, cardToUpdate);
            await UpdateExpirationDateAsync(updateCardDto, cardToUpdate);
            await UpdateCurrencyAsync(updateCardDto, cardToUpdate, currency);

            return cardToUpdate;
        }

        private static Task UpdateExpirationDateAsync(UpdateCardDto updateCardDto, Card cardToUpdate)
        {
            if (updateCardDto.ExpirationDate != null)
            {
                cardToUpdate.ExpirationDate = (DateTime)updateCardDto.ExpirationDate;
            }

            return Task.FromResult(true);
        }
        private static Task UpdateCardTypeAsync(UpdateCardDto updateCardDto, Card cardToUpdate)
        {
            if (!string.IsNullOrEmpty(updateCardDto.CardType))
            {
                if (Enum.TryParse(updateCardDto.CardType, out CardType parsedCardType))
                {
                    cardToUpdate.CardType = parsedCardType;
                }
            }
            return Task.FromResult(true);
        }
        private static Task UpdateCurrencyAsync(UpdateCardDto updateCardDto, Card cardToUpdate, Currency currency)
        {
            if (updateCardDto.CurrencyCode != null)
            {
                cardToUpdate.CurrencyId = currency.Id;
                cardToUpdate.Currency.CurrencyCode = currency.CurrencyCode;
            }

            return Task.FromResult(true);
        }
    }
}
