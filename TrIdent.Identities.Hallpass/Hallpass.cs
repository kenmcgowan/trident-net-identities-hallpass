using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TrIdent.Identities
{
   public sealed class Hallpass : IIdentity
   {
      #region fields
      private static Regex ValidNamePattern = new Regex(
         @"^\s*\S+\s*$",
         RegexOptions.Compiled | RegexOptions.CultureInvariant);
      private static Regex ValidValuePattern = new Regex(
          @"^\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2}-\d{6} \S+ \S+ \d+$",
          RegexOptions.Compiled | RegexOptions.CultureInvariant);
      private static Random InsecureRng = new Random();
      private string _value;
      #endregion

      #region instantiation
      private Hallpass(string value)
      {
         this._value = value;
      }

      public Hallpass(string issuer, string recipient)
      {
         if (issuer == null)
         {
            throw new ArgumentNullException(nameof(issuer));
         }

         if (recipient == null)
         {
            throw new ArgumentNullException(nameof(recipient));
         }

         if (!ValidNamePattern.IsMatch(issuer))
         {
            throw new ArgumentException("Invalid issuer value", nameof(issuer));
         }

         if (!ValidNamePattern.IsMatch(recipient))
         {
            throw new ArgumentException("Invalid recipient value", nameof(recipient));
         }

         var normalizedIssuer = issuer.Trim().ToLowerInvariant();
         var normalizedRecipient = recipient.Trim().ToLowerInvariant();

         _value = string.Format(
             "{0:yyyy-MM-dd-HH-mm-ss-ffffff} {1} {2} {3}",
             DateTime.UtcNow,
             normalizedIssuer,
             normalizedRecipient,
             InsecureRng.Next());
      }
      #endregion

      #region operators
      public static explicit operator string(Hallpass identity)
      {
         if (identity == null)
         {
            return null;
         }

         return identity._value;
      }

      public static explicit operator Hallpass(string source)
      {
         if (source == null)
         {
            return null;
         }

         if (!ValidValuePattern.IsMatch(source))
         {
            throw new ArgumentException(
               "Invalid hallpass identity value",
               nameof(source));
         }

         GetValueTimestamp(source);

         try
         {
            GetNumericalSuffix(source);
         }
         catch (ArgumentException argumentException)
         {
            throw new ArgumentException(
               "Invalid hallpass identity value: Invalid numerical suffix",
               nameof(source),
               argumentException);
         }

         return new Hallpass(source);
      }

      public static explicit operator byte[] (Hallpass identity)
      {
         if (identity == null)
         {
            return null;
         }

         return Encoding.UTF8.GetBytes(identity._value);
      }

      public static explicit operator Hallpass(byte[] bytes)
      {
         if (bytes == null)
         {
            return null;
         }

         string value;

         try
         {
            value = Encoding.UTF8.GetString(bytes);
            return (Hallpass)value;
         }
         catch (ArgumentException argumentException)
         {
            throw new ArgumentException(
               "Invalid hallpass identity byte sequence",
               nameof(bytes),
               argumentException);
         }
         catch (Exception exception)
         {
            throw new ArgumentException(
               "Invalid hallpass identity byte sequence - unknown exception",
               nameof(bytes),
               exception);
         }
      }
      #endregion

      #region overrides
      public override bool Equals(object obj)
      {
         return (this == (obj as Hallpass));
      }

      public override int GetHashCode()
      {
         return GetNumericalSuffix(this._value);
      }
      #endregion

      #region IIdentity implementation
      public bool Equals(IIdentity other)
      {
         if (other == null)
         {
            return false;
         }

         var otherHallpass = other as Hallpass;

         if (otherHallpass == null)
         {
            return false;
         }

         return (string.Compare(
            this._value,
            otherHallpass._value,
            false,
            CultureInfo.InvariantCulture) == 0);
      }
      #endregion

      #region private methods
      private static DateTime GetValueTimestamp(string source)
      {
         string s;
         int year;
         int month;
         int day;
         int hour24;
         int minute;
         int second;
         int microsecond;

         try
         {
            s = source.Substring(0, 4);
            year = int.Parse(s);

            s = source.Substring(5, 2);
            month = int.Parse(s);

            s = source.Substring(8, 2);
            day = int.Parse(s);

            s = source.Substring(11, 2);
            hour24 = int.Parse(s);

            s = source.Substring(14, 2);
            minute = int.Parse(s);

            s = source.Substring(17, 2);
            second = int.Parse(s);

            s = source.Substring(20, 6);
            microsecond = int.Parse(s);
         }
         catch (Exception exception)
         {
            throw new ArgumentException(
                "Invalid hallpass identity value: invalid timestamp - bad date/time format",
                nameof(source),
                exception);
         }

         try
         {
            return new DateTime(year, month, day, hour24, minute, second, microsecond / 1000);
         }
         catch (ArgumentOutOfRangeException argumentOutOfRangeException)
         {
            throw new ArgumentException(
                "Invalid hallpass identity value: invalid timestamp - invalid date/time value",
                nameof(source),
                argumentOutOfRangeException);
         }
         catch (Exception exception)
         {
            throw new ArgumentException(
                "Invalid hallpass identity value: invalid timestamp - unknown exception",
                nameof(source),
                exception);
         }
      }

      private static int GetNumericalSuffix(string identityValue)
      {
         int spaceIndex;

         // This finds the space separating the issuer from the recipient.
         spaceIndex = identityValue.IndexOf(' ', 27);

         // Based on the last index, this then finds the last space after the recipient value.
         spaceIndex = identityValue.IndexOf(' ', spaceIndex + 1);

         var rndString = identityValue.Substring(spaceIndex + 1);

         int rndSuffix;

         if (!int.TryParse(rndString, out rndSuffix))
         {
            throw new ArgumentException(
               "Inavalid numerical suffix",
               nameof(identityValue));
         }

         return rndSuffix;
      }
      #endregion
   }
}
