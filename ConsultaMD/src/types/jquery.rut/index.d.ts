/// <reference types="jquery"/>

interface ICallback {
    (r: number, dv: string): void;
}

interface JQueryStatic {
    validateRut(
        rut: string,
        callback: any,
        object: { minimumLength: number }) : boolean;
}

interface JQuery<TElement = HTMLElement> {
    rut(object: {
        formatOn: string,
        minimumLength?: number,
        validateOn?: string
    }): this;
}