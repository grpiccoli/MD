// The default export is currently a legacy one.
// (containing legacy functions along with the new API).
// `/min`, `/max`, `/mobile` sub-packages are for the new API only.

declare namespace libphonenumber {
    class PhoneNumber {
        constructor(countryCallingCodeOrCountry: CountryCallingCode | CountryCode, nationalNumber: NationalNumber, metadata: Metadata);
        countryCallingCode: CountryCallingCode;
        country?: CountryCode;
        nationalNumber: NationalNumber;
        number: E164Number;
        carrierCode?: CarrierCode;
        ext?: Extension;
        isPossible(): boolean;
        isValid(): boolean;
        getType(): NumberType;
        format(format: NumberFormat, options?: FormatNumberOptions): string;
        formatNational(options?: FormatNumberOptionsWithoutIDD): string;
        formatInternational(options?: FormatNumberOptionsWithoutIDD): string;
        getURI(options?: FormatNumberOptionsWithoutIDD): string;
    }

    type FormatExtension = (formattedNumber: string, extension: Extension, metadata: Metadata) => string

    type FormatNumberOptionsWithoutIDD = {
        v2?: boolean;
        formatExtension?: FormatExtension;
    };

    type FormatNumberOptions = {
        v2?: boolean;
        fromCountry?: CountryCode;
        humanReadable?: boolean;
        formatExtension?: FormatExtension
    };

    type NumberFormat = 'NATIONAL' | 'National' | 'INTERNATIONAL' | 'International' | 'E.164' | 'RFC3966' | 'IDD';
    type NumberType = undefined | 'PREMIUM_RATE' | 'TOLL_FREE' | 'SHARED_COST' | 'VOIP' | 'PERSONAL_NUMBER' | 'PAGER' | 'UAN' | 'VOICEMAIL' | 'FIXED_LINE_OR_MOBILE' | 'FIXED_LINE' | 'MOBILE';

    interface Extension extends String { }

    interface CarrierCode extends String { }

    type CountryCallingCodes = {
        [countryCallingCode: string]: CountryCode[];
    };

    interface E164Number extends String { }

    type Metadata = {
        country_calling_codes: CountryCallingCodes;
        countries: Countries;
    };

    type Countries = {
        [country in CountryCode]: any[];
    };

    interface NationalNumber extends String { }

    interface CountryCallingCode extends String { }

    type CountryCode = '001' | 'AC' | 'AD' | 'AE' | 'AF' | 'AG' | 'AI' | 'AL' | 'AM' | 'AO' | 'AR' | 'AS' | 'AT' | 'AU' | 'AW' | 'AX' | 'AZ' | 'BA' | 'BB' | 'BD' | 'BE' | 'BF' | 'BG' | 'BH' | 'BI' | 'BJ' | 'BL' | 'BM' | 'BN' | 'BO' | 'BQ' | 'BR' | 'BS' | 'BT' | 'BW' | 'BY' | 'BZ' | 'CA' | 'CC' | 'CD' | 'CF' | 'CG' | 'CH' | 'CI' | 'CK' | 'CL' | 'CM' | 'CN' | 'CO' | 'CR' | 'CU' | 'CV' | 'CW' | 'CX' | 'CY' | 'CZ' | 'DE' | 'DJ' | 'DK' | 'DM' | 'DO' | 'DZ' | 'EC' | 'EE' | 'EG' | 'EH' | 'ER' | 'ES' | 'ET' | 'FI' | 'FJ' | 'FK' | 'FM' | 'FO' | 'FR' | 'GA' | 'GB' | 'GD' | 'GE' | 'GF' | 'GG' | 'GH' | 'GI' | 'GL' | 'GM' | 'GN' | 'GP' | 'GQ' | 'GR' | 'GT' | 'GU' | 'GW' | 'GY' | 'HK' | 'HN' | 'HR' | 'HT' | 'HU' | 'ID' | 'IE' | 'IL' | 'IM' | 'IN' | 'IO' | 'IQ' | 'IR' | 'IS' | 'IT' | 'JE' | 'JM' | 'JO' | 'JP' | 'KE' | 'KG' | 'KH' | 'KI' | 'KM' | 'KN' | 'KP' | 'KR' | 'KW' | 'KY' | 'KZ' | 'LA' | 'LB' | 'LC' | 'LI' | 'LK' | 'LR' | 'LS' | 'LT' | 'LU' | 'LV' | 'LY' | 'MA' | 'MC' | 'MD' | 'ME' | 'MF' | 'MG' | 'MH' | 'MK' | 'ML' | 'MM' | 'MN' | 'MO' | 'MP' | 'MQ' | 'MR' | 'MS' | 'MT' | 'MU' | 'MV' | 'MW' | 'MX' | 'MY' | 'MZ' | 'NA' | 'NC' | 'NE' | 'NF' | 'NG' | 'NI' | 'NL' | 'NO' | 'NP' | 'NR' | 'NU' | 'NZ' | 'OM' | 'PA' | 'PE' | 'PF' | 'PG' | 'PH' | 'PK' | 'PL' | 'PM' | 'PR' | 'PS' | 'PT' | 'PW' | 'PY' | 'QA' | 'RE' | 'RO' | 'RS' | 'RU' | 'RW' | 'SA' | 'SB' | 'SC' | 'SD' | 'SE' | 'SG' | 'SH' | 'SI' | 'SJ' | 'SK' | 'SL' | 'SM' | 'SN' | 'SO' | 'SR' | 'SS' | 'ST' | 'SV' | 'SX' | 'SY' | 'SZ' | 'TA' | 'TC' | 'TD' | 'TG' | 'TH' | 'TJ' | 'TK' | 'TL' | 'TM' | 'TN' | 'TO' | 'TR' | 'TT' | 'TV' | 'TW' | 'TZ' | 'UA' | 'UG' | 'US' | 'UY' | 'UZ' | 'VA' | 'VC' | 'VE' | 'VG' | 'VI' | 'VN' | 'VU' | 'WF' | 'WS' | 'XK' | 'YE' | 'YT' | 'ZA' | 'ZM' | 'ZW';

    function parsePhoneNumberFromString(text: string, defaultCountry?: CountryCode): PhoneNumber | undefined;

    class AsYouType {
        constructor(defaultCountryCode?: CountryCode);
        input(text: string): string;
        reset(): void;
        country: CountryCode | undefined;
        getNumber(): PhoneNumber | undefined;
        getNationalNumber(): string;
        getTemplate(): string | undefined;
    }
}