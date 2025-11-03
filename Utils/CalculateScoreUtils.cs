namespace DuploBackend.Utils;

public static class CalculateScoreUtils
{
    public static decimal CalculateCreditScore(decimal totalAmount, int totalOrders)
    {
        if (totalOrders == 0)
            return 0;

        return totalAmount / (totalOrders * 100);
    }
}
