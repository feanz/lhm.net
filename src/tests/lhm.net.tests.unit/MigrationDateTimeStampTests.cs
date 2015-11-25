using System;
using Should.Fluent;
using Xunit;

namespace lhm.net.tests.unit
{
    public class MigrationDateTimeStampTests
    {
        [Fact]
        public void Should_not_require_value_at_construction()
        {
            var sut = new MigrationDateTimeStamp();

            sut.ToString()
                .Should()
                .Not
                .Be
                .Empty();
        }

        [Fact]
        public void Should_throw_exception_if_constructed_with_invalid_arguments()
        {
            Assert.Throws<ArgumentException>(() => new MigrationDateTimeStamp("Foo"));
        }

        [Fact]
        public void Should_construct_with_valid_value() => new MigrationDateTimeStamp(Valid.Samples.ValidMigrationDateTimeStamp);

        public class TryParse
        {
            [Theory]
            [InlineData(Valid.Samples.ValidMigrationDateTimeStamp, true)]
            [InlineData(Invalid.Samples.InvalidYearFormat, false)]
            [InlineData(Invalid.Samples.SingleDigitMonthFormat, false)] 
            [InlineData(Invalid.Samples.SingleDigetDayFormat, false)] 
            [InlineData(Invalid.Samples.SingleDigitHourFormat, false)] 
            [InlineData(Invalid.Samples.SingleDigitMinutes, false)] 
            [InlineData(Invalid.Samples.SingleDigitSeconds, false)] 
            [InlineData(Invalid.Samples.IncorrectNumberOfMiliseconds, false)] 
            [InlineData(Invalid.Samples.JustPlainRubbish, false)]
            [InlineData(Invalid.Samples.InvalidMonth, false)]
            [InlineData(Invalid.Samples.InvalidDays, false)]
            [InlineData(Invalid.Samples.InvalidHours, false)]
            [InlineData(Invalid.Samples.InvalidMinutes, false)]
            [InlineData(Invalid.Samples.InvalidSeconds, false)]
            public void Should_be_true_for_valid_value(string candidate, bool isValid)
            {
                MigrationDateTimeStamp temp;
                var actual = MigrationDateTimeStamp.TryParse(candidate, out temp);

                actual.Should().Equal(isValid);
            }
        }

        public class IsValid
        {
            [Theory]
            [InlineData(Valid.Samples.ValidMigrationDateTimeStamp, true)]
            [InlineData(Invalid.Samples.InvalidYearFormat, false)]
            [InlineData(Invalid.Samples.SingleDigitMonthFormat, false)]
            [InlineData(Invalid.Samples.SingleDigetDayFormat, false)]
            [InlineData(Invalid.Samples.SingleDigitHourFormat, false)]
            [InlineData(Invalid.Samples.SingleDigitMinutes, false)]
            [InlineData(Invalid.Samples.SingleDigitSeconds, false)]
            [InlineData(Invalid.Samples.IncorrectNumberOfMiliseconds, false)]
            [InlineData(Invalid.Samples.JustPlainRubbish, false)]
            [InlineData(Invalid.Samples.InvalidMonth, false)]
            [InlineData(Invalid.Samples.InvalidDays, false)]
            [InlineData(Invalid.Samples.InvalidHours, false)]
            [InlineData(Invalid.Samples.InvalidMinutes, false)]
            [InlineData(Invalid.Samples.InvalidSeconds, false)]
            public void Should_be_true_for_valid_value(string candidate, bool isValid)
            {
                var actual = MigrationDateTimeStamp.IsValid(candidate);

                actual.Should().Equal(isValid);
            }
        }

        public class Equality
        {
            [Fact]
            public void Should_be_true_for_matching_stamps()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;

                var one = new MigrationDateTimeStamp(stamp);
                var two = new MigrationDateTimeStamp(stamp);

                one.Equals(two).Should().Be.True();
            }

            [Fact]
            public void Should_be_false_for_non_matching_stamps()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;

                var one = new MigrationDateTimeStamp(stamp);
                var two = new MigrationDateTimeStamp("2015_01_01_15_12_14_999");

                one.Equals(two).Should().Be.False();
            }

            [Fact]
            public void Should_be_false_null()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;

                var one = new MigrationDateTimeStamp(stamp);

                one.Equals(null).Should().Be.False();
            }

            [Fact]
            public void Should_be_false_for_non_matching_string()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;

                var one = new MigrationDateTimeStamp(stamp);

                one.Equals("foo").Should().Be.False();
            }

            [Fact]
            public void Should_be_true_for_matching_string()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;

                var one = new MigrationDateTimeStamp(stamp);

                one.Equals(stamp).Should().Be.True();
            }
        }

        public class Stamp
        {
            [Fact]
            public void Should_add_stamp_to_the_end_of_candidate_value()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;
                var sut = new MigrationDateTimeStamp(stamp);

                var actual = sut.Stamp("candidate");

                var expected = $"candidate_{stamp}";

                actual.Should().Equal(expected);
            }

            [Fact]
            public void Should_swap_timestamp_candidate_value_already_end_one()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;
                var sut = new MigrationDateTimeStamp(stamp);

                var candidate = "candidate_2015_05_05_15_15_15_151";

                var actual = sut.Stamp(candidate);
                
                actual.Should().Equal($"candidate_{stamp}");
            }

            [Fact]
            public void Should_not_swap_timestamp_value_if_its_not_at_the_end_of_the_string()
            {
                var stamp = Valid.Samples.ValidMigrationDateTimeStamp;
                var sut = new MigrationDateTimeStamp(stamp);

                var candidate = "2015_05_05_15_15_15_151_candidate";

                var actual = sut.Stamp(candidate);

                actual.Should().Equal($"2015_05_05_15_15_15_151_candidate_{stamp}");
            }
        }

        public class ContainsMigrationDateTime
        {
            [Theory]
            [InlineData(Valid.Samples.ValidMigrationDateTimeStamp, true)]
            [InlineData(Invalid.Samples.InvalidYearFormat, false)]
            [InlineData(Invalid.Samples.SingleDigitMonthFormat, false)]
            [InlineData("foo2015_05_05_15_15_15_151bar", true)]
            [InlineData("2015_05_05_15_15_15_151bar", true)]
            [InlineData("foo2015_05_05_15_15_15_151", true)]
            [InlineData("foo2015_05_05_15_15_151", false)]
            [InlineData("foo2015_05_05_15_15_151bar", false)]
            public void Should_add_stamp_to_the_end_of_candidate_value(string candidate, bool isValid)
            {
                var actual = MigrationDateTimeStamp.ContainsMigrationDateTime(candidate);
                
                actual.Should().Equal(isValid);
            }
        }

        private static class Invalid
        {
            public static class Samples
            {
                public const string InvalidYearFormat = "1_01_01_15_12_14_876";
                public const string SingleDigitMonthFormat = "2015_1_01_15_12_14_876";
                public const string SingleDigetDayFormat = "2015_01_1_15_12_14_876";
                public const string SingleDigitHourFormat = "2015_01_01_7_12_14_876";
                public const string SingleDigitMinutes = "2015_01_01_15_1_6_876";
                public const string SingleDigitSeconds = "2015_01_01_15_12_4_876";
                public const string IncorrectNumberOfMiliseconds = "2015_01_01_15_12_14_8";
                public const string InvalidMonth = "2015_13_01_15_12_14_876";
                public const string InvalidDays = "2015_13_32_15_12_14_876";
                public const string InvalidHours = "2015_13_32_25_12_14_876";
                public const string InvalidMinutes = "2015_13_32_25_61_14_876";
                public const string InvalidSeconds = "2015_13_32_25_12_61_876";
                public const string JustPlainRubbish = "Rubbish";
            }
        }

        private static class Valid
        {
            public static class Samples
            {
                public const string ValidMigrationDateTimeStamp = "2015_01_01_15_12_14_876";
            }
        }

    }
}