declare class Cleave {
    constructor(element: any, options: CleaveOptions);
}

declare interface CleaveOptions {
    delimiter?: string;
    numeral?: boolean;
    blocks?: number[];
    numeralThousandsGroupStyle?: string;
    uppercase?: boolean;
}