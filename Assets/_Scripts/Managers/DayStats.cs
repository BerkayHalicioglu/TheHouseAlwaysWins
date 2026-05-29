public struct DayStats
{
    public int cheatersCaught;
    public int innocentsSent;
    public int cheatersMissed;
    public int correctReleases;
    public float bankrollStart;
    public float bankrollEnd;

    public int TotalDecisions   => cheatersCaught + innocentsSent + cheatersMissed + correctReleases;
    public int CorrectDecisions => cheatersCaught + correctReleases;
    public float NetProfit      => bankrollEnd - bankrollStart;

    public float EfficiencyPercent =>
        TotalDecisions > 0 ? (float)CorrectDecisions / TotalDecisions * 100f : 100f;

    public string EfficiencyRating
    {
        get
        {
            float e = EfficiencyPercent;
            if (e >= 90f) return "S";
            if (e >= 75f) return "A";
            if (e >= 60f) return "B";
            if (e >= 45f) return "C";
            return "D";
        }
    }

    public string EfficiencyRatingColor
    {
        get
        {
            float e = EfficiencyPercent;
            if (e >= 90f) return "#FFD700"; // altın
            if (e >= 75f) return "#00FF88"; // yeşil
            if (e >= 60f) return "#FFFFFF"; // beyaz
            if (e >= 45f) return "#FFA500"; // turuncu
            return "#FF4444";               // kırmızı
        }
    }
}
