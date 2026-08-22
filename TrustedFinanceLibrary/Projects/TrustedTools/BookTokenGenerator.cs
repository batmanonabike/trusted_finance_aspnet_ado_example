namespace TrustedTools
{
    public static class BookTokenGenerator
    {
        public static string RandomGenre() => Pick(Genres);
        public static decimal RandomPrice() => Random.Shared.Next(499, 5000) / 100m; // (4.99 > 49.99)

        public static string RandomTitle()
        {
            return Random.Shared.Next(5) switch
            {
                0 => $"The {Pick(TitleAdjectives)} {Pick(TitleNouns)}",
                1 => $"{Pick(TitleNouns)} of {Pick(TitlePlurals)}",
                2 => $"The {Pick(TitleNouns)} of {Pick(TitlePlurals)}",
                3 => $"A {Pick(TitleAdjectives)} {Pick(TitleNouns)}",
                _ => $"{Pick(TitleAdjectives)} {Pick(TitlePlurals)}"
            };
        }

        public static string RandomAuthor()
        {
            return Random.Shared.Next(4) switch
            {
                0 => $"{Pick(FirstNames)} {Pick(Surnames)}",
                1 => $"{Pick(FirstNames)} {RandomInitial()}. {Pick(Surnames)}",
                2 => $"{RandomInitial()}. {RandomInitial()}. {Pick(Surnames)}",
                _ => $"{Pick(FirstNames)} {Pick(Surnames)}-{Pick(Surnames)}"
            };
        }

        public static DateOnly RandomPublishDate()
        {
            var latest = DateOnly.FromDateTime(DateTime.Today);
            var earliest = latest.AddYears(-50);
            return earliest.AddDays(Random.Shared.Next(latest.DayNumber - earliest.DayNumber + 1));
        }

        private static char RandomInitial() => (char)('A' + Random.Shared.Next(26));
        private static string Pick(string[] seeds) => seeds[Random.Shared.Next(seeds.Length)];

        private static readonly string[] TitleAdjectives =
        [
            "Secured", "Compound", "Leveraged", "Prudent", "Distressed", "Liquid",
            "Sovereign", "Amortised", "Hedged", "Unsecured", "Overdrawn", "Solvent"
        ];

        private static readonly string[] TitleNouns =
        [
            "Ledger", "Portfolio", "Mortgage", "Dividend", "Covenant", "Broker",
            "Guarantor", "Yield", "Premium", "Underwriter", "Remortgage", "Bondholder"
        ];

        private static readonly string[] TitlePlurals =
        [
            "Arrears", "Assets", "Debts", "Markets", "Creditors", "Margins",
            "Repayments", "Defaults", "Bulls", "Bears"
        ];

        private static readonly string[] FirstNames =
        [
            "Martyn", "Margaret", "Nadine", "Harry", "Robert", "Gail", "Richard", "Desmond",
            "Nadia", "Alex", "Imogen", "Rupert", "Alexandra", "Eleanor"
        ];

        private static readonly string[] Surnames =
        [
            "Brown", "Rosu", "Rodgers", "Smith", "Green",
            "Peacock", "Conroy", "Dickinson", "Trusted", "Thorne"
        ];

        private static readonly string[] Genres =
        [
            "Finance", "Economics", "Business", "Investment",
            "Accounting", "Property", "Biography", "Reference"
        ];
    }
}
