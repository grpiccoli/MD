/// <reference types="jquery"/>

interface AnyOptions {
    mode?: string;
    parent?: string;
    layout?: string;
    hAlign?: string;
    vAlign?: string;
    lang?: string;
    rtl?: boolean;
    animationDuration?: number;
    theme: string;
}

interface JQuery<TElement = HTMLElement> {
    AnyPicker(options: AnyOptions): this;
}