declare interface _IMobiscroll {
    settings: any;
    $: any;
    i18n: any;
    fw: string;
    apiKey?: string;
    apiUrl?: string;
    uid?: string;
    util: IMobiscrollUtils;
    activeInstance?: any;
    platform: {
        name: string;
        majorVersion: number;
        minorVersion: number;
    };
    customTheme: (name: string, baseTheme: string) => void;
}

declare interface IMobiscrollUtils { }

declare interface MbscCoreOptions {
    // Settings
    cssClass?: string;
    theme?: string;
    lang?: string;
    rtl?: boolean;
    responsive?: object;
    tap?: boolean;

    // Events
    onInit?(event: any, inst: any): void;
    onDestroy?(event: any, inst: any): void;
}

declare class Base<T extends MbscCoreOptions> {
    settings: T;

    constructor(element: any, settings: T);

    init(settings?: T): void;
    destroy(): void;
    tap(el: any, handler: (ev?: any, inst?: any) => void, prevent?: boolean, tolerance?: number, time?: any): void;
    trigger(name: string, event?: any): any;
    option(options: string | T, value?: any): void;
    getInst(): Base<T>;
}

declare interface MbscFrameOptions extends MbscCoreOptions {
    // Settings
    anchor?: string | HTMLElement;
    animate?: boolean | 'fade' | 'flip' | 'pop' | 'swing' | 'slidevertical' | 'slidehorizontal' | 'slidedown' | 'slideup';
    buttons?: Array<any>;
    closeOnOverlayTap?: boolean;
    context?: string | HTMLElement;
    disabled?: boolean;
    display?: 'top' | 'bottom' | 'bubble' | 'inline' | 'center';
    focusOnClose?: boolean | string | HTMLElement;
    focusTrap?: boolean;
    headerText?: string | boolean | ((formattedValue: string) => string);
    layout?: 'liquid' | 'fixed';
    scrollLock?: boolean;
    showOnFocus?: boolean;
    showOnTap?: boolean;
    showOverlay?: boolean;
    touchUi?: boolean;
    labelStyle?: 'stacked' | 'inline' | 'floating';
    inputStyle?: 'underline' | 'box' | 'outline';

    // Events
    onBeforeClose?(event: { valueText: string, button: string }, inst: any): void;
    onBeforeShow?(event: any, inst: any): void;
    onCancel?(event: { valuteText: string }, inst: any): void;
    onClose?(event: { valueText: string }, inst: any): void;
    onDestroy?(event: any, inst: any): void;
    onFill?(event: any, inst: any): void;
    onMarkupReady?(event: { target: HTMLElement }, inst: any): void;
    onPosition?(event: { target: HTMLElement, windowWidth: number, windowHeight: number }, inst: any): void;
    onShow?(event: { target: HTMLElement, valueText: string }, inst: any): void;

}

declare interface MbscDataControlOptions {
    select?: 'single' | 'multiple' | number;
}

declare type MbscDataFrameOptions = MbscDataControlOptions & MbscFrameOptions;

declare class Frame<T extends MbscFrameOptions> extends Base<T> {
    buttons: object;
    handlers: {
        set: () => void,
        cancel: () => void,
        clear: () => void
    };
    _value: any;
    _isValid: boolean;
    _isVisible: boolean;

    position(check?: boolean): void;
    attachShow(elm: any, beforeShow?: () => void): void;
    select(): void;
    cancel(): void;
    clear(): void;
    enable(): void;
    disable(): void;
    show(prevAnim?: boolean, prevFocus?: boolean): void;
    hide(prevAnim?: boolean, btn?: string, force?: boolean, callback?: () => void): void;
    isVisible(): boolean;
}

declare interface MbscScrollerOptions extends MbscFrameOptions {
    // Settings
    circular?: boolean | Array<boolean>;
    height?: number;
    maxWidth?: number | Array<number>;
    minWidth?: number | Array<number>;
    multiline?: number;
    readOnly?: boolean | Array<boolean>;
    rows?: number;
    showLabel?: boolean;
    showScrollArrows?: boolean;
    wheels?: {
        [index: number]: {
            [index: number]: {
                label?: string,
                key?: string | number,
                circular?: boolean,
                cssClass?: string,
                data: Array<string | { display: string, value: any }> | ((index: number) => string | { display: string, value: any }),
                getIndex?: (value: any) => number
            }
        }
    };
    width?: number | Array<number>;

    // Events
    onChange?(event: { valueText?: string }, inst: any): void;
    validate?(data: { values: Array<any>, index: number, direction: number }, inst: any): (void | { disabled?: Array<any>, valid?: Array<any> });
    onSet?(event: { valueText?: string }, inst: any): void;
    onItemTap?(event: any, inst: any): void;
    onClear?(event: any, inst: any): void;

    // localization
    cancelText?: string;
    clearText?: string;
    selectedText?: string;
    setText?: string;

    formatValue?(data: Array<any>): string;
    parseValue?(valueText: string): any;
}

declare class Scroller<T extends MbscScrollerOptions = MbscScrollerOptions> extends Frame<T> {
    setVal(value: any, fill?: boolean, change?: boolean, temp?: boolean, time?: number): void;
    getVal(temp?: boolean): any;
    setArrayVal(value: any, fill?: boolean, change?: boolean, temp?: boolean, time?: number): void;
    getArrayVal(temo?: boolean): any;
    changeWheel(
        wheels: {
            [index: number]: {
                label?: string,
                key?: string | number,
                circular?: boolean,
                cssClass?: string,
                data: Array<string | { display: string, value: any }> | ((index: number) => string | { display: string, value: any }),
                getIndex?: (value: any) => number
            },
            [index: string]: {
                label?: string,
                key?: string | number,
                circular?: boolean,
                cssClass?: string,
                data: Array<string | { display: string, value: any }> | ((index: number) => string | { display: string, value: any }),
                getIndex?: (value: any) => number
            }
        },
        time: number,
        manual?: boolean): void;
    getValidValue(index?: number, val?: any, dir?: any, dis?: boolean): any;
}

declare interface MbscCalbaseOptions extends MbscDatetimeOptions {
    // Settings
    calendarHeight?: number;
    calendarWidth?: number;
    calendarScroll?: 'horizontal' | 'vertical';
    counter?: boolean;
    defaultValue?: any | Array<any>;
    labels?: Array<{ start?: any, end?: any, d?: string | any, text?: string, color?: string, background?: string, cssClass?: string }>;
    events?: Array<{ start?: any, end?: any, d?: string | any, text?: string, color?: string, background?: string, cssClass?: string }>;
    marked?: Array<any | number | string | { d: any | number | string, color?: string, background?: string, cssClass?: string }>;
    colors?: Array<{ d: any | number | string, background?: string, cssClass?: string }>;
    months?: number | 'auto';
    mousewheel?: boolean;
    outerMonthChange?: boolean;
    showOuterDays?: boolean;
    tabs?: boolean;
    weekCounter?: 'year' | 'month';
    weekDays?: 'full' | 'short' | 'min';
    weeks?: number;
    yearChange?: boolean;

    // localization
    dateText?: string;
    dayNamesMin?: Array<string>;
    firstDay?: number;
    timeText?: string;
    moreEventsPluralText?: string;
    moreEventsText?: string;

    // Events
    onTabChange?(event: { tab: 'calendar' | 'date' | 'time' }, inst: any): void;
    onDayChange?(event: { date: Date, marked?: any, selected?: 'start' | 'end', target: HTMLElement }, inst: any): void;
    onLabelTap?(event: { date: Date, domEvent: any, target: HTMLElement, labels?: any[], label?: any }, inst: any): void;
    onMonthChange?(event: { year: number, month: number }, inst: any): void;
    onMonthLoading?(event: { year: number, month: number }, inst: any): void;
    onMonthLoaded?(event: { year: number, month: number }, inst: any): void;
    onPageChange?(event: { firstDay: Date, lastDay?: Date }, inst: any): void;
    onPageLoading?(event: { firstDay: Date, lastDay?: Date }, inst: any): void;
    onPageLoaded?(event: { firstDay: Date, lastDay?: Date }, inst: any): void;
    onSetDate?(event: { date: Date, control: 'calendar' | 'date' | 'time' }, inst: any): void;
}

declare class CalBase<T extends MbscCalbaseOptions> extends DatetimeBase<T> {
    refresh(): void;
    redraw(): void;
    navigate(d: Date, anim?: boolean): void;
    changeTab(tab: 'calendar' | 'date' | 'time'): void;
}

declare interface MbscCalendarOptions extends MbscCalbaseOptions {
    // Settings
    controls?: Array<'time' | 'date' | 'calendar'>;
    firstSelectDay?: number;
    selectType?: 'day' | 'week';
    select?: 'single' | 'multiple' | number;
    setOnDayTap?: boolean;

    // Events
    onSetDate?(event: { date: Date, control?: 'calendar' | 'date' | 'time' }, inst: any): void;
}

declare class Calendar extends CalBase<MbscCalendarOptions> {
    getVal(temp?: boolean): any | Array<any>;
    setVal(value: any | Array<any>, fill?: boolean, change?: boolean, temp?: boolean): void;
}

declare interface MbscDatetimeOptions extends MbscScrollerOptions {
    // Settings
    defaultValue?: any | Array<any>;
    invalid?: Array<string | { start: any, end: any, d?: string } | any>;
    max?: any;
    min?: any;
    returnFormat?: 'iso8601' | 'moment' | 'locale' | 'jsdate';
    steps?: { hour?: number, minute?: number, second?: number, zeroBased?: boolean };
    valid?: Array<string | { start: any, end: any, d?: string } | any>;
    calendarSystem?: 'jalali' | 'hijri' | 'gregorian';
    moment?: any;

    // localization
    ampmText?: string;
    amText?: string;
    dateFormat?: string;
    dateWheels?: string;
    dayNames?: Array<string>;
    dayNamesShort?: Array<string>;
    dayText?: string;
    hourText?: string;
    minuteText?: string;
    monthNames?: Array<string>;
    monthNamesShort?: Array<string>;
    monthSuffix?: string;
    monthText?: string;
    nowText?: string;
    pmText?: string;
    secText?: string;
    timeFormat?: string;
    timeWheels?: string;
    yearSuffix?: string;
    yearText?: string;
}

declare class DatetimeBase<T extends MbscDatetimeOptions = MbscDatetimeOptions> extends Scroller<T> {
    handlers: {
        set: () => void,
        cancel: () => void,
        clear: () => void
        now: () => void
    };

    getDate(temp?: boolean): Date | null;
    setDate(date: Date, fill?: boolean, time?: number, temp?: boolean, change?: boolean): void;
}

declare interface MbscRangeOptions extends MbscCalbaseOptions {
    // Settings
    autoCorrect?: boolean;
    controls?: Array<'time' | 'date' | 'calendar'>;
    endInput?: string | HTMLElement;
    maxRange?: number;
    minRange?: number;
    showSelector?: boolean;
    startInput?: string | HTMLElement;

    // localization
    fromText?: string;
    toText?: string;

    // Events
    onSetDate?(event: { date: Date, active: 'start' | 'end', control: 'calendar' | 'date' | 'time' }, inst: any): void;
}

declare class RangePicker extends CalBase<MbscRangeOptions> {
    startVal: string;
    endVal: string;
    setVal(values: Array<any>, fill?: boolean, change?: boolean, temp?: boolean, time?: number): void;
    getVal(temp?: boolean): Array<any>;
    setActiveDate(active: 'start' | 'end'): void;
}

declare interface IMobiscroll extends _IMobiscroll {
    //card(selector: string | HTMLElement, options?: MbscCardOptions): Card;
    calendar(selector: string | HTMLElement, options?: MbscCalendarOptions): Calendar;
    //color(selector: string | HTMLElement, options?: MbscColorOptions): Color;
    //datetime(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    //date(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    //time(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    //eventcalendar(selector: string | HTMLElement, options?: MbscEventcalendarOptions): Eventcalendar;
    //form(selector: string | HTMLElement, options?: MbscFormOptions): Form;
    //page(selector: string | HTMLElement, options?: MbscPageOptions): Page;
    //image(selector: string | HTMLElement, options?: MbscImageOptions): ImageScroller;
    //listview(selector: string | HTMLElement, options?: MbscListviewOptions): ListView;
    //optionlist(selector: string | HTMLElement, options?: MbscOptionlistOptions): Optionlist;
    //measurement(selector: string | HTMLElement, options?: MbscMeasurementOptions): Measurement;
    //mass(selector: string | HTMLElement, options?: MbscMassOptions): Mass;
    //distance(selector: string | HTMLElement, options?: MbscDistanceOptions): Distance;
    //force(selector: string | HTMLElement, options?: MbscForceOptions): Force;
    //speed(selector: string | HTMLElement, options?: MbscSpeedOptions): Speed;
    //temperature(selector: string | HTMLElement, options?: MbscTemperatureOptions): Temperature;
    //nav(selector: string | HTMLElement, options?: MbscNavOptions): Navigation;
    //number(selector: string | HTMLElement, options?: MbscNumberOptions): NumberScroller;
    //numpad(selector: string | HTMLElement, options?: MbscNumpadOptions): Numpad;
    range(selector: string | HTMLElement, options?: MbscRangeOptions): RangePicker;
    //scroller(selector: string | HTMLElement, options?: MbscScrollerOptions): Scroller;
    //scrollview(selector: string | HTMLElement, options?: MbscScrollViewOptions): ScrollView;
    //select(selector: string | HTMLElement, options?: MbscSelectOptions): Select;
    //timer(selector: string | HTMLElement, options?: MbscTimerOptions): Timer;
    //timespan(selector: string | HTMLElement, options?: MbscTimespanOptions): Timespan;
    //treelist(selector: string | HTMLElement, options?: MbscTreelistOptions): Treelist;
    //popup(selector: string | HTMLElement, options?: MbscPopupOptions): Popup;
    //widget(selector: string | HTMLElement, options?: MbscWidgetOptions): Widget;
}

declare const mobiscroll: IMobiscroll;