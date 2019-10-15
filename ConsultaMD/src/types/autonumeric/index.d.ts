declare class AutoNumeric {
    constructor(element: any, options: ANOptions);
}

declare interface ANOptions {
    allowDecimalPadding: boolean;
    decimalCharacter: string;
    decimalPlaces: number;
    decimalPlacesRawValue: number;
    decimalPlacesShownOnBlur: number;
    decimalPlacesShownOnFocus: number;
    digitGroupSeparator: string;
    maximumValue: string;
    minimumValue: string;
    modifyValueOnWheel: boolean;
}