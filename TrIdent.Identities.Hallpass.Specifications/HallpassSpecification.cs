using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using TrIdent.Specifications;
using Xunit;

namespace TrIdent.Identities.Specifications
{
   public class HallpassSpecification : IIdentitySpecification<Hallpass>
   {
      #region ctor tests
      [Fact]
      public void Ctor_WhenGivenANullIssuer_ThrowsAnArgumentNullException()
      {
         string nullIssuer = null;
         var validRecipient = "Ed";

         Action callToCreateWithNullIssuer = () => new Hallpass(nullIssuer, validRecipient);

         callToCreateWithNullIssuer.Should().ThrowExactly<ArgumentNullException>()
             .Where(argumentNullException => argumentNullException.ParamName == "issuer");
      }

      [Theory]
      [MemberData(nameof(InvalidIssuerOrRecipientValueArguments))]
      public void Ctor_WhenGivenAnInvalidIssuer_ThrowsAnArgumentException(string invalidIssuer)
      {
         var validRecipient = "Alan";

         Action callToCreateWithInvalidIssuer = () => new Hallpass(invalidIssuer, validRecipient);

         callToCreateWithInvalidIssuer.Should().ThrowExactly<ArgumentException>()
             .Where(argumentException => argumentException.ParamName == "issuer");
      }

      [Fact]
      public void Ctor_WhenGivenANullRecipient_ThrowsAnArgumentNullException()
      {
         var validIssuer = "Mr._Horton";
         string nullRecipient = null;

         Action callToCreateWithNullRecipient = () => new Hallpass(validIssuer, nullRecipient);

         callToCreateWithNullRecipient.Should().ThrowExactly<ArgumentNullException>()
             .Where(argumentNullException => argumentNullException.ParamName == "recipient");
      }

      [Theory]
      [MemberData(nameof(InvalidIssuerOrRecipientValueArguments))]
      public void Ctor_WhenGivenAnInvalidRecipient_ThrowsAnArgumentException(string invalidRecipient)
      {
         var validIssuer = "Mr._Hassan";

         Action callToCreateWithInvalidIssuer = () => new Hallpass(validIssuer, invalidRecipient);

         callToCreateWithInvalidIssuer.Should().ThrowExactly<ArgumentException>()
             .Where(argumentException => argumentException.ParamName == "recipient");
      }

      [Theory]
      [MemberData(nameof(ValidIssuerAndRecipientValueArguments))]
      public void Ctor_WhenGivenValidArguments_Succeeds(string validIssuer, string validRecipient)
      {
         Action constructorCallWithValidArguments = () => new Hallpass(validIssuer, validRecipient);

         constructorCallWithValidArguments.Should().NotThrow();
      }
      #endregion

      #region override tests
      [Fact]
      public void GetHashCode_ShouldReturnTheSameValueOnSubsequentCalls()
      {
         var sut = new Hallpass("Mr._Z", "Rick");

         var theFirstResult = sut.GetHashCode();
         var theSecondResult = sut.GetHashCode();

         theFirstResult.Should().Be(theSecondResult);
      }

      [Fact]
      public void GetHashCode_ShouldReturnDifferentValuesForDifferentInstances()
      {
         var sut1 = new Hallpass("Mr._Lentz", "John");
         var sut2 = new Hallpass("Mrs.VanVleck", "Chrissy");

         sut1.GetHashCode().Should().NotBe(sut2.GetHashCode());
      }
      #endregion

      #region operator tests
      [Fact]
      public void ExplicitCastFromStringToHallpass_GivenANullString_YieldsNull()
      {
         string aNullString = null;

         var hallpassCastFromNullString = (Hallpass)aNullString;

         hallpassCastFromNullString.Should().BeNull();
      }

      [Fact]
      public void ExplicitCastFromHallpassToString_GivenANullHallpass_YieldsNull()
      {
         Hallpass aNullHallpass = null;

         var stringCastFromNullHallpass = (string)aNullHallpass;

         stringCastFromNullHallpass.Should().BeNull();
      }

      [Theory]
      [MemberData(nameof(InvalidIdentityValueArguments))]
      public void ExplicitCastFromStringToHallpass_GivenAnInvalidIdentityValue_ThrowsAnArgumentException(string invalidIdentityValue)
      {
         Action invalidCastFromStringToHallpass = () => { var x = (Hallpass)invalidIdentityValue; };

         invalidCastFromStringToHallpass.Should().ThrowExactly<ArgumentException>()
            .Where(argumentException => argumentException.ParamName == "source");
      }

      [Theory]
      [MemberData(nameof(ValidIdentityValueArguments))]
      public void ExplicitCastFromStringToHallpass_GivenAValidIdentityValue_Succeeds(string validIdentityValue)
      {
         Action validCastFromStringToHallpass = () => { var x = (Hallpass)validIdentityValue; };

         validCastFromStringToHallpass.Should().NotThrow();
      }

      [Fact]
      public void ExplicitCastFromHallpassToString_ShouldYieldANonemptyString()
      {
         var sut = new Hallpass("Kitch", "Moss");

         var result = (string)sut;

         result.Should().NotBeNullOrWhiteSpace();
      }

      [Fact]
      public void ExplicitCastFromHallassToString_ShouldCastBackToAnEquivalentHallpass()
      {
         var sut = new Hallpass("Some_issuer", "Some_recipient");
         var identityString = (string)sut;
         var dehydratedHallpass = (Hallpass)identityString;
         var otherHallpass = new Hallpass("Some_other_issuer", "Some_other_recipient");
         var sutEqualsDehydratedHallpass = sut.Equals(dehydratedHallpass);
         var sutEqualsSomeOtherHallpass = sut.Equals(otherHallpass);

         using (new AssertionScope())
         {
            sutEqualsDehydratedHallpass.Should().BeTrue();
            sutEqualsSomeOtherHallpass.Should().BeFalse();
         }
      }

      [Fact]
      public void ExplicitCastFromByteArrayToHallpass_GivenANullByteArray_YieldsNull()
      {
         byte[] aNullByteArray = null;

         var hallpassCastFromNullByteArray = (Hallpass)aNullByteArray;

         hallpassCastFromNullByteArray.Should().BeNull();
      }

      [Fact]
      public void ExplicitCastFromHallpassToByteArray_GivenANullHallpass_YieldsNull()
      {
         Hallpass aNullHallpass = null;

         var byteArrayCastFromNullHallpass = (byte[])aNullHallpass;

         byteArrayCastFromNullHallpass.Should().BeNull();
      }

      [Fact]
      public void ExplicitCastFromHallpassToByteArray_ShouldYieldANonemptyByteArray()
      {
         var sut = new Hallpass("Teacher", "Student");

         var result = (byte[])sut;

         result.Should().NotBeNull();
         result.Length.Should().BeGreaterThan(0);
      }

      [Fact]
      public void ExplicitCastFromHallassToByteArray_ShouldCastBackToAnEquivalentHallpass()
      {
         var sut = new Hallpass("Puff", "Bob");
         var identityBytes = (byte[])sut;
         var dehydratedHallpass = (Hallpass)identityBytes;
         var otherHallpass = new Hallpass("Squidward", "Patrick");
         var sutEqualsDehydratedHallpass = sut.Equals(dehydratedHallpass);
         var sutEqualsSomeOtherHallpass = sut.Equals(otherHallpass);

         using (new AssertionScope())
         {
            sutEqualsDehydratedHallpass.Should().BeTrue();
            sutEqualsSomeOtherHallpass.Should().BeFalse();
         }
      }

      [Fact]
      public void ExplicitCastFromByteArrayToHallpass_GivenAnEmptyByteArray_ThrowsAnArgumentException()
      {
         var emptyByteArray = new byte[0];
         Action castFromEmptyByteArrayToHallpass = () =>
         {
            var x = (Hallpass)emptyByteArray;
         };

         castFromEmptyByteArrayToHallpass.Should().ThrowExactly<ArgumentException>()
            .Where(argumentException => argumentException.ParamName == "bytes");
      }

      [Fact]
      public void ExplicitCastFromByteArrayToHallpass_GivenAByteArrayFullOfJunk_ThrowsAnArgumentException()
      {
         var junkByteArray = new byte[] { 106, 117, 110, 107 };
         Action castFromJunkByteArrayToHallpass = () =>
         {
            var x = (Hallpass)junkByteArray;
         };

         castFromJunkByteArrayToHallpass.Should().ThrowExactly<ArgumentException>()
            .Where(argumentException => argumentException.ParamName == "bytes");
      }

      // Invalid byte arrays yield exceptions (empty, random bytes)
      #endregion

      #region test infrastructure
      protected override Hallpass CreateSubject()
      {
         return new Hallpass("Mr._Mohenke", "Ken");
      }
      #endregion

      #region test data
      private static readonly string[] ValidIssuerOrRecipientValues = new[] { "Mr._Moehnke", "Mr._Z", "Mrs._Reilly", "Mr._Lentz", "Ken", "April", "Joy", "Sean", "Michelle ", " Joe", "\tRolf", "\rKeith\n" };
      private static readonly string[] InvalidIssuerAndRecipientValues = new[] { "", " ", "\t", "\r\n", "\t ", "\r\n  \t", "no spaces", "no\tabs", "no\r\nlinebreaks" };
      private static readonly string[] InvalidIdentityValues = new[]
      {
         "",
         " ",
         "2018-14-02-04-14-12-395702 nonexistent month 19587",
         "2018-10-32-04-22-37-789154 nonexistent day 598235",
         "2018-06-20-47-48-27-045461 nonexistent hour 3466",
         "2018-10-32-04-70-55-734168 nonexistent minute 4753567",
         "2018-10-32-04-19-98-157498 nonexistent second 34564357",
         "1971-02-29-01-18-43-481987 phantom leapyear 39850983",
         "18-10-22-15-57-36-985093 twodigit year 938598",
         "2018-10-20-17-49-30-054109 too many names 65465198",
         "2018-10-20-17-49-30-054109 oversized suffix 456878945415159878456515980540984015651608706",
         "2016-09-01-19-20-21-654088 no suffix ",
         "1000-13-97-68-61-72-156087 it's all wrong "
      };
      private static readonly string[] ValidIdentityValues = new[]
      {
         "1970-11-05-21-27-02-540498 Judy Ken 465404568",
         "1969-08-01-19-32-45-480987 Andrea April 870691",
         "1988-06-01-11-32-15-544109 WhoKnows Someone 901987156"
      };

      public static IEnumerable<object[]> ValidIssuerAndRecipientValueArguments
      {
         get
         {
            // This somewhat odd syntax returns the cross product of the "valid values" array with itself (i.e., all possible value combination pairs).
            return ValidIssuerOrRecipientValues.SelectMany(
               issuer => ValidIssuerOrRecipientValues,
               (issuer, recipient) => new object[] { issuer, recipient });
         }
      }

      public static IEnumerable<object[]> InvalidIssuerOrRecipientValueArguments
      {
         get
         {
            return InvalidIssuerAndRecipientValues.Select(s => new object[] { s });
         }
      }

      public static IEnumerable<object[]> InvalidIdentityValueArguments
      {
         get
         {
            return InvalidIdentityValues.Select(s => new object[] { s });
         }
      }

      public static IEnumerable<object[]> ValidIdentityValueArguments
      {
         get
         {
            return ValidIdentityValues.Select(s => new object[] { s });
         }
      }
      #endregion
   }
}
