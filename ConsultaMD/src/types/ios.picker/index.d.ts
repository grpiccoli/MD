/// <reference types="jquery"/>

interface ICallback {
    (s: number): void;
}

interface JQuery<TElement = HTMLElement> {
    picker(object: {
        data: string[],
        selected?: number,
        lineHeight?: number
    },
    callback: ICallback | null): this;
}