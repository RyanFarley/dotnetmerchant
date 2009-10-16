#region License

// The MIT License
// 
// Copyright (c) 2009 Conatus Creative, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace DotNetMerchant.Model
{
    /// <summary>
    /// A seamless currency class for working with money. 
    /// This class uses runtime information to separate how currency is displayed 
    /// to a user from the native currency format itself. You can create instances
    /// of CurrencyInfo implicitly using instances of <see cref="Currency"/>,
    /// <see cref="DisplayCulture"/>, or <see cref="RegionInfo" />.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Code}")]
    public partial class CurrencyInfo
    {
        private static readonly IDictionary<string, CultureInfo> _cultures;

        static CurrencyInfo()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.FrameworkCultures)
                .Where(c => !c.IsNeutralCulture &&
                            !c.ThreeLetterISOLanguageName.Equals("IVL"));

            _cultures = new Dictionary<string, CultureInfo>(0);
            foreach (var culture in cultures)
            {
                _cultures.Add(culture.Name, culture);
            }
        }

        private CurrencyInfo()
        {
        }

        public string DisplayName { get; private set; }
        public Currency Code { get; private set; }

        /// <summary>
        /// The native region where this currency is from.
        /// </summary>
        public RegionInfo NativeRegion { get; private set; }

        /// <summary>
        /// The display culture set when this currency instance was created. 
        /// It reflects the best guess between the thread of the culture the instance
        /// was created on, and the native region of the currency itself.
        /// </summary>
        public CultureInfo DisplayCulture { get; private set; }

        public bool Equals(CurrencyInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return ReferenceEquals(this, other) || Equals(other.Code, Code);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.GetType() == typeof (CurrencyInfo) && Equals((CurrencyInfo) other);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static bool operator ==(CurrencyInfo left, CurrencyInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyInfo left, CurrencyInfo right)
        {
            return !Equals(left, right);
        }

        public static implicit operator CurrencyInfo(Currency currency)
        {
            var currencyInfo = _currencies[currency];
            currencyInfo.NativeRegion = GetNativeRegionFromCurrencyCodeAndUserCulture(currency);
            currencyInfo.DisplayCulture = GetDisplayCultureFromCurrencyCodeAndUserCulture(currency);

            return currencyInfo;
        }

        public static implicit operator CurrencyInfo(RegionInfo regionInfo)
        {
            var symbol = regionInfo.ISOCurrencySymbol;
            var currencyCode = (Currency) Enum.Parse(typeof (Currency), symbol);

            return currencyCode;
        }

        public static implicit operator CurrencyInfo(CultureInfo cultureInfo)
        {
            return new RegionInfo(cultureInfo.LCID);
        }

        private static RegionInfo GetNativeRegionFromCurrencyCodeAndUserCulture(Enum currencyCode)
        {
            // Get the current culture and region
            var userCulture = CultureInfo.CurrentCulture;
            var userLanguageName = userCulture.TwoLetterISOLanguageName;
            var userRegionName = new RegionInfo(userCulture.LCID).TwoLetterISORegionName;
            var currencySymbol = currencyCode.ToString("G");

            // Get all regions with the given currency (pivot on language to avoid losing precision)
            var locales = (from c in _cultures.Values
                           let r = new RegionInfo(c.LCID)
                           where r.ISOCurrencySymbol.Equals(currencySymbol)
                           select new {Region = r, Culture = c});

            // Resolve the native region to the one the user is in, or the first valid one
            var locale = locales.SingleOrDefault(
                             l => l.Region.TwoLetterISORegionName.Equals(userRegionName) &&
                                  l.Culture.TwoLetterISOLanguageName.Equals(userLanguageName))
                         ?? locales.FirstOrDefault();

            return locale.Region;
        }

        private static CultureInfo GetDisplayCultureFromCurrencyCodeAndUserCulture(Enum currencyCode)
        {
            var userCulture = CultureInfo.CurrentCulture;
            var languageCode = userCulture.TwoLetterISOLanguageName;
            var nativeRegion = GetNativeRegionFromCurrencyCodeAndUserCulture(currencyCode);

            var cultureName = string.Format("{0}-{1}", languageCode, nativeRegion);
            var cultureInfo = _cultures.ContainsKey(cultureName)
                                  ? new CultureInfo(cultureName)
                                  : new CultureInfo(languageCode);

            return cultureInfo;
        }
    }
}