/// <reference types="jquery"/>

declare interface IParsleyConfig {
    trigger?: string;
    successClass?: string;
    errorClass?: string;
    classHandler?: any;
    errorsWrapper?: string;
    errorTemplate?: string;
    errorsContainer?: any;
}

interface JQuery<TElement = HTMLElement> {
    parsley(options?: IParsleyConfig): this;
    addValidator(name: string, options: ParsleyOptions): void;
    addError(name: string, options: ErrorOptions): this;
    validate(): this;
    isValid(): boolean;
    reset(): this;
}

declare interface ErrorOptions {
    message: string;
    updateClass: boolean;
}

declare interface ParsleyOptions {
    validateString?: any;
    validateNumber?: any;
    validateDate?: any;
    validateMultiple?: any;
    messages?: any;
}

declare const Parsley: JQuery<HTMLElement>;