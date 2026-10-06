namespace zogo.Domain.Enums;

public enum BuyerPropertyInteractionStatus : short
{
    Interested = 1,
    VisitRequested = 2,
    Visited = 3,
    DocumentsRequested = 4,
    Negotiating = 5,
    OfferSubmitted = 6,
    OfferAccepted = 7,
    OfferRejected = 8,
    NotInterested = 9
}
