using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace lhm.net
{
    public class MigrationDateTimeStamp
    {
        private static readonly string EndsWithDatetimeStampPattern = $"{DatetimeStampPattern}$";
        private const string DatetimeStampPattern = @"\d{4}_\d{2}_\d{2}_\d{2}_\d{2}_\d{2}_\d{3}";
        private const string DateTimeStampFormat = "yyyy_MM_dd_HH_mm_ss_fff";

        private readonly string _migrationDateTime;

        /// <summary>
        /// Create a migration dateTime stamp if no stamp value is provided one is generated from the current UTC dateTime
        /// </summary>
        /// <param name="migrationDateTime"></param>
        public MigrationDateTimeStamp(string migrationDateTime = null)
        {
            if (migrationDateTime != null && !IsValid(migrationDateTime))
                throw new ArgumentException("Invalid value", nameof(migrationDateTime));

            _migrationDateTime = migrationDateTime ?? DateTime.UtcNow.ToString(DateTimeStampFormat);
        }

        /// <summary>
        /// Add a migration datetime stamp to the end of a string in this format '{candidate}_{valueToStamp}.  If the value already end in a migration datetime stamp 
        /// then replace that value with the new one provided
        /// </summary>
        public string Stamp(string candidate)
        {
            return EndWithMigrationDateTime(candidate) ? 
                Regex.Replace(candidate, DatetimeStampPattern, _migrationDateTime) :
                $"{candidate}_{_migrationDateTime}";
        }

        /// <summary>
        /// Determines if a string contains a migration format datetime stamp
        /// </summary>
        public static bool ContainsMigrationDateTime(string candidate)
        {
            var match = Regex.Match(candidate, DatetimeStampPattern);
            return match.Success;
        }

        public static bool IsValid(string candidate)
        {
            if (string.IsNullOrEmpty(candidate))
                return false;

            DateTime temp;
            return DateTime.TryParseExact(candidate, DateTimeStampFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out temp);
        }

        public static bool TryParse(string candidate, out MigrationDateTimeStamp migrationDateTimeStamp)
        {
            migrationDateTimeStamp = null;
            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            if (IsValid(candidate))
            {
                migrationDateTimeStamp = new MigrationDateTimeStamp(candidate);
                return true;
            }
            return false;
        }

        public static implicit operator string (MigrationDateTimeStamp migrationDateTimeStamp)
        {
            return migrationDateTimeStamp._migrationDateTime;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj.ToString() == _migrationDateTime;
        }

        public override int GetHashCode()
        {
            return _migrationDateTime.GetHashCode();
        }

        public override string ToString()
        {
            return _migrationDateTime;
        }

        private bool EndWithMigrationDateTime(string candidate)
        {
            var match = Regex.Match(candidate, EndsWithDatetimeStampPattern);
            return match.Success;
        }
    }
}