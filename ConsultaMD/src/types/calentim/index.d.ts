/// <reference types="jquery"/>
/// <reference types="moment"/>

interface CalentimOptions {
    inline?: boolean;
    calendarCount?: number;
    startDate?: moment.Moment;
    endDate?: moment.Moment;
    format?: string;
    hourFormat?: 12 | 24;
    minuteSteps?: number;
    minDate?: moment.Moment;
    maxDate?: moment.Moment;
    showHeader?: boolean;
    showFooter?: boolean;
    showButtons?: boolean;
    hideOutOfRange?: boolean;
    enableKeyboard?: boolean;
    startOnMonday?: boolean;
    container?: string;
    oneCalendarWidth?: number;
    shownOn?: "bottom" | "top" | "left" | "right" | "center";
    arrowOn?: "bottom" | "top" | "left" | "right" | "center";
    autoAlign?: boolean;
    locale?: string;
    singleDate?: boolean;
    target?: JQuery<HTMLElement>;
    ranges?: any[];
    rangeLabel?: string;
    cancelLabel?: string;
    applyLabel?: string;
    enableMonthSwitcher?: boolean;
    enableYearSwitcher?: boolean;
    numericMonthSwitcher?: boolean;
    monthSwitcherFormat?: string;
    showWeekNumbers?: boolean;
    autoCloseOnSelect?: boolean;
    rangeOrientation?: 'vertical' | 'horizontal';
    ontimechange?: any;
    onafterselect?: any;
}

interface CalentimObject {

}

interface JQuery<TElement = HTMLElement> {
    calentim(options?: CalentimOptions): CalentimObject;
}