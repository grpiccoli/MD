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

declare interface IMobiscrollUtils {
    getJson(url:string, callback:any, type:string): void;
}

declare interface MbscCoreOptions {
    // Settings
    cssClass?: string;
    theme?: string;
    themeVariant?: string;
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

declare interface MbscNumpadOptions extends MbscFrameOptions {
    // Settings
    allowLeadingZero?: boolean;
    deleteIcon?: string;
    fill?: 'ltr' | 'rtl';
    leftKey?: { text: string, variable?: string, value?: string };
    mask?: string;
    placeholder?: string;
    rightKey?: { text: string, variable?: string, value?: string };
    template?: string;
    preset?: 'date' | 'time' | 'decimal' | 'timespan';

    // Events
    onSet?(event: { valueText: string }, inst: any): void;
    validate?(data: { values: Array<any>, variables: any }, inst: any): ({ disabled?: Array<any>, invalid?: boolean });
    onClear?(event: any, inst: any): void;

    // localization
    cancelText?: string;
    clearText?: string;
    setText?: string;

    formatValue?(numbers: Array<any>, variables: any, inst: any): string;
    parseValue?(valueText: string): any;
}

declare interface MbscNumpadDecimalOptions extends MbscNumpadOptions {
    decimalSeparator?: string;
    defaultValue?: number;
    invalid?: Array<any>;
    scale?: number;
    min?: number;
    max?: number;
    prefix?: string;
    preset?: 'decimal';
    returnAffix?: boolean;
    suffix?: string;
    thousandsSeparator?: string;
}

declare interface MbscNumpadDateOptions extends MbscNumpadOptions {
    dateFormat?: string;
    dateOrder?: string;
    delimiter?: string;
    defaultValue?: string;
    invalid?: Array<any>;
    min?: Date;
    max?: Date;
    preset?: 'date';
}

declare interface MbscNumpadTimeOptions extends MbscNumpadOptions {
    defaultValue?: string;
    invalid?: Array<any>;
    max?: Date;
    min?: Date;
    preset?: 'time';
    timeFormat?: string;
}

declare interface MbscNumpadTimespanOptions extends MbscNumpadOptions {
    defaultValue?: number;
    invalid?: Array<any>;
    min?: number;
    max?: number;
    preset?: 'timespan';
}

declare class Numpad extends Frame<MbscNumpadOptions> {
    setVal(val: string | number | Date, fill?: boolean, change?: boolean, temp?: boolean): void;
    getVal(temp?: boolean): string | number | Date;
    setArrayVal(val: Array<any>, fill?: boolean, change?: boolean, temp?: boolean): void;
    getArrayVal(temp?: boolean): Array<any>;
}

declare interface MbscMeasurementBaseOptions extends MbscScrollerOptions {
    // Settings
    max?: number;
    min?: number;
    defaultValue?: string;
    invalid?: Array<any>;
    scale?: number;
    step?: number;
    units?: Array<string>;

    // localization
    wholeText?: string;
    fractionText?: string;
    signText?: string;
}

declare interface MbscMeasurementOptions extends MbscMeasurementBaseOptions {
    // Settings
    convert?(val: number, unit1: string, unit2: string): number;
}

declare class Measurement<T extends MbscMeasurementBaseOptions = MbscMeasurementOptions> extends Scroller<T> { }

declare interface MbscNumberOptions extends MbscMeasurementBaseOptions { }

declare class NumberScroller extends Measurement<MbscNumberOptions> { }

declare interface MbscCardOptions extends MbscCoreOptions {
    collapsible?: boolean
}

declare class Card extends Base<MbscCardOptions> {
    // methods
    refresh(shallow?: boolean): void;
    toggle?(): void;
    hide?(): void;
    show?(): void;
}

declare interface MbscColorOptions extends MbscDataFrameOptions {
    // Settings
    clear?: boolean;
    data?: Array<string | { color: string }>;
    defaultValue?: string;
    enhance?: boolean;
    format?: 'hex' | 'rgb' | 'hsl';
    inputClass?: string;
    mode?: 'preset' | 'refine';
    navigation?: 'horizontal' | 'vertical';
    preview?: boolean;
    previewText?: boolean;
    rows?: number;
    valueText?: string;

    // Events
    onSet?(event: { valueText: string }, inst: any): void;
    onClear?(event: any, inst: any): void;
    onItemTap?(event: { target: HTMLElement, selected: boolean, index: number, value: string }, inst: any): void;
    onPreviewItemTap?(event: { target: HTMLElement, index: number, value: string }, inst: any): void;
}


declare class Color extends Frame<MbscColorOptions> {
    setVal(val: string | Array<string>, fill?: boolean, change?: boolean, temp?: boolean): void;
    getVal(temp?: boolean): string | Array<string>;
}

declare class DateTime extends DatetimeBase { }

declare type MbscDataCalbaseOptions = MbscDataControlOptions & MbscCalbaseOptions;

declare interface MbscEventcalendarOptions extends MbscDataCalbaseOptions {
    // Settings
    data?: Array<{ start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }>;
    eventBubble?: boolean;
    formatDuration?(start: Date, end: Date): string;
    showEventCount?: boolean;
    view?: {
        calendar?: { type?: 'week' | 'month', size?: number, popover?: boolean, labels?: boolean },
        eventList?: { type?: 'day' | 'week' | 'month' | 'year', size?: number, scrollable?: boolean }
    };
    // Localization
    allDayText?: string;
    noEventsText?: string;
    eventText?: string;
    eventsText?: string;
    labelsShort?: Array<string>;
    // Events
    onEventSelect?(event: { event: any, date: Date, domEvent: any }, inst: Eventcalendar): void;
    onDayChange?(event: { events: Array<{ start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }>, date: Date, marked?: any, selected?: 'start' | 'end', target: HTMLElement }, inst: any): void;
}

declare class Eventcalendar extends CalBase<MbscEventcalendarOptions> {
    navigate(d: Date, anim?: boolean, pop?: boolean): void;
    addEvent(events: Array<{ start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }> | { start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }): Array<number | string>;
    updateEvent(event: { start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }): void;
    removeEvent(eids: Array<string | number>): void;
    getEvents(d: Date): Array<{ start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }>;
    setEvents(events: Array<{ start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }> | { start?: any, end?: any, d?: any | string | number, text?: string, color?: string, allDay?: boolean }): Array<number | string>;
}

declare interface MbscFormOptions extends MbscCoreOptions {
    inputStyle?: string;
    labelStyle?: string;
    enhance?: boolean;
    context?: string | HTMLElement;
}

declare class FormControl {
    settings: MbscFormOptions;

    constructor(element: any, settings: MbscFormOptions);

    destroy(): void;
    option(settings: MbscFormOptions): void;
    handleEvent(event: any): void;
}

declare class Input extends FormControl {
    constructor(element: any, settings: MbscFormOptions);
    refresh(): void;
}

declare class Form extends Base<MbscFormOptions> {
    refresh(shallow?: boolean): void;
}

declare interface MbscPageOptions extends MbscCoreOptions {
    // Settings
    context?: string | HTMLElement;
}

declare class Page extends Base<MbscPageOptions> { }

declare interface MbscImageOptions extends MbscScrollerOptions {
    defaultValue?: Array<number | string>;
    enhance?: boolean;
    inputClass?: string;
    invalid?: Array<any>;
    labels?: Array<string>;
    placeholder?: string;
    showInput?: boolean;
}

declare class List<T extends MbscScrollerOptions> extends Scroller<T> {
}

declare class ImageScroller extends List<MbscImageOptions> {
}

declare interface MbscListviewOptions extends MbscCoreOptions {
    actions?: Array<any> | { left?: any, right?: any };
    actionsWidth?: number;
    context?: string | HTMLElement;
    striped?: boolean;
    animateIcons?: boolean;
    animateAddRemove?: boolean;
    actionable?: boolean;
    enhance?: boolean;
    fillAnimation?: boolean;
    fixedHeader?: boolean;
    hover?: 'left' | 'right' | { direction?: 'left' | 'right', time?: number, timeout?: number };
    iconSlide?: boolean;
    itemGroups?: any;
    navigateOnDrop?: boolean;
    quickSwipe?: boolean;
    sortable?: boolean | { group?: boolean, handle?: boolean | 'left' | 'right', multilevel?: boolean };
    sortDelay?: number;
    stages?: Array<any> | { left?: Array<any>, right?: Array<any> };
    swipe?: boolean | 'left' | 'right' | ((args: { target: HTMLElement, index: number, direction: 'left' | 'right' }, inst: any) => (boolean | 'left' | 'right'));
    swipeleft?(): void;
    swiperight?(): void;
    vibrate?: boolean;
    loadingIcon?: string;
    select?: 'off' | 'single' | 'multiple';

    // localization
    undoText?: string;
    backText?: string;

    // Events
    onItemTap?(event: { target: HTMLElement, domEvent: any, index: number }, inst: any): void;
    onItemAdd?(event: { target: HTMLElement }, inst: any): void;
    onItemRemove?(event: { target: HTMLElement }, inst: any): void;
    onNavEnd?(event: { level: number, direction: 'left' | 'right', list: HTMLElement }, inst: any): void;
    onNavStart?(event: { level: number, direction: 'left' | 'right', list: HTMLElement }, inst: any): void;
    onSlideEnd?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSlideStart?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSort?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSortChange?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSortStart?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSortEnd?(event: { target: HTMLElement, index: number }, inst: any): void;
    onSortUpdate?(event: { target: HTMLElement, index: number }, inst: any): void;
    onStageChange?(event: { target: HTMLElement, index: number, stage: any }, inst: any): void;
    onListEnd?(event: {}, inst: any): void;
}

declare class ListView extends Base<MbscListviewOptions> {
    animate(li: any, anim: string, callback?: (ul: any, li: any) => void): void;
    add(id: string | number, markup: string, index?: number, callback?: (ul: any, li: any) => void, parent?: any, isUndo?: boolean): void;
    swipe(li: any, percent: number, time?: number, demo?: boolean, callback?: () => void): void;
    openStage(li: any, stage: string, time?: number, demo?: boolean): void;
    openActions(li: any, dir: 'left' | 'right', time?: number, demo?: boolean): void;
    close(li: any, time?: number): void;
    remove(li: any, dir?: 'left' | 'right', callback?: (ul: any, li: any) => void, isUndo?: boolean): void;
    move(li: any, index: number, direction?: 'left' | 'right', callback?: (ul: any, li: any) => void, parent?: any, isUndo?: boolean): void;
    navigate(li: any, callback?: (ul: any, li: any) => void): void;
    startActionTrack(): void;
    endActionTrack(): void;
    addUndoAction(action: any, async?: boolean): void;
    undo(): void;
    hideLoading(): void;
    showLoading(): void;
    select(li: any): void;
    deselect(li: any): void;
}

declare interface MbscMeasurementBaseOptions extends MbscScrollerOptions {
    // Settings
    max?: number;
    min?: number;
    defaultValue?: string;
    invalid?: Array<any>;
    scale?: number;
    step?: number;
    units?: Array<string>;

    // localization
    wholeText?: string;
    fractionText?: string;
    signText?: string;
}

declare interface MbscMeasurementOptions extends MbscMeasurementBaseOptions {
    // Settings
    convert?(val: number, unit1: string, unit2: string): number;
}

declare interface MbscScrollViewOptions extends MbscCoreOptions {
    // settings
    context?: string | HTMLElement;
    itemWidth?: number;
    layout?: 'liquid' | 'fixed' | number;
    mousewheel?: boolean;
    paging?: boolean;
    snap?: boolean;
    threshold?: number;

    // Events
    onItemTap?(event: { target: HTMLElement }, inst: any): void;
    onMarkupReady?(event: { target: HTMLElement }, inst: any): void;
    onAnimationStart?(event: any, inst: any): void;
    onAnimationEnd?(event: any, inst: any): void;
    onMove?(event: any, inst: any): void;
    onGestureStart?(event: any, inst: any): void;
    onGestureEnd?(event: any, inst: any): void;
}

declare class ScrollView<T extends MbscScrollViewOptions = MbscScrollViewOptions> extends Base<T> {
    navigate(item: any, toggle?: boolean): void;
    next(toggle?: boolean): void;
    prev(toggle?: boolean): void;
    refresh(noScroll?: boolean): void;
}

declare interface MbscNavBaseOptions extends MbscScrollViewOptions {
    display?: 'top' | 'bottom' | 'inline';
}

declare class NavigationBase<T extends MbscNavBaseOptions = MbscNavBaseOptions> extends ScrollView<T> {
    select(item: any): void;
    deselect(item: any): void;
    enable(item: any): void;
    disable(item: any): void;
    setBadge(item: any, content: string): void;
}

declare interface MbscOptionlistOptions extends MbscNavBaseOptions {
    // Settings
    paging?: boolean;
    select?: 'single' | 'multiple' | 'off';
    snap?: boolean;
    type?: 'options' | 'tabs' | 'menu';
    variant?: 'a' | 'b';
    threshold?: number;
}

declare class Optionlist extends NavigationBase<MbscOptionlistOptions> { }

declare interface MbscTemperatureOptions extends MbscMeasurementBaseOptions {
    // Settings
    convert?: boolean;
    defaultUnit?: string;
    unitNames?: any;
}

declare class Temperature extends Measurement<MbscTemperatureOptions> { }

declare interface MbscMassOptions extends MbscTemperatureOptions { }

declare class Mass extends Measurement<MbscMassOptions> { }

declare interface MbscDistanceOptions extends MbscTemperatureOptions { }

declare class Distance extends Measurement<MbscDistanceOptions> { }

declare interface MbscForceOptions extends MbscTemperatureOptions { }

declare class Force extends Measurement<MbscForceOptions> { }

declare interface MbscSpeedOptions extends MbscTemperatureOptions { }

declare class Speed extends Measurement<MbscSpeedOptions> { }

declare interface MbscNavOptions extends MbscNavBaseOptions {
    type: 'bottom' | 'hamburger' | 'tab';
    moreText?: string;
    moreIcon?: string;
    menuText?: string;
    menuIcon?: string;
    onMenuHide?(event: { target: HTMLElement, menu: any }, inst: any): void;
    onMenuShow?(event: { target: HTMLElement, menu: any }, inst: any): void;
}

declare class Navigation extends NavigationBase<MbscNavOptions> { }

declare interface MbscNumberOptions extends MbscMeasurementBaseOptions { }

declare type MbscDataScrollerOptions = MbscDataControlOptions & MbscScrollerOptions;

declare  interface MbscSelectOptions extends MbscDataScrollerOptions {
    // Settings
    counter?: boolean;
    data?: Array<{ text?: string, value?: any, group?: string, html?: string, disabled?: boolean }> | {
        url: string,
        dataField?: string,
        dataType?: 'json' | 'jsonp',
        processResponse?: (data: any) => Array<{ text?: string, value?: any, group?: string, html?: string, disabled?: boolean }>,
        remoteFilter?: boolean
    };
    dataText?: string;
    dataGroup?: string;
    dataValue?: string;
    filter?: boolean;
    filterPlaceholderText?: string;
    filterEmptyText?: string;
    group?: boolean | { header?: boolean, groupWheel?: boolean, clustered?: boolean };
    groupLabel?: string;
    input?: string | object;
    inputClass?: string;
    invalid?: Array<any>;
    label?: string;
    placeholder?: string;
    showInput?: boolean;
    onFilter?(event: { filterText: string }, inst: any): false | void;
}

declare class Select extends Scroller<MbscSelectOptions> {
    setVal(val: string | number | Array<string | number>, fill?: boolean, change?: boolean, temp?: boolean, time?: number): void;
    getVal(temp?: boolean, group?: boolean): string | number | Array<string | number>;
    refresh(data?: Array<{ text?: string, value?: any, group?: string, html?: string, disabled?: boolean }>, filter?: string, callback?: () => void): void;
}

declare interface MbscTimerOptions extends MbscScrollerOptions {
    // Settings
    autostart?: boolean;
    maxWheel?: 'years' | 'months' | 'days' | 'hours' | 'minutes' | 'seconds' | 'fract';
    mode?: 'countdown' | 'stopwatch';
    step?: number;
    targetTime?: number;
    useShortLabels?: boolean;

    // localization
    hideText?: string;
    labels?: Array<string>;
    labelsShort?: Array<string>;
    lapText?: string;
    resetText?: string;
    startText?: string;
    stopText?: string;

    // Events
    onLap?(event: { ellapsed: number, lap: number, laps: Array<number> }, inst: any): void;
    onFinish?(event: { time: number }, inst: any): void;
    onReset?(event: any, inst: any): void;
    onStart?(event: any, inst: any): void;
    onStop?(event: { ellapsed: number }, inst: any): void;
}

declare class Timer extends Scroller<MbscTimerOptions> {
    handlers: {
        set: () => void,
        cancel: () => void,
        clear: () => void
        toggle: () => void,
        start: () => void,
        stop: () => void,
        resetLap: () => void,
        reset: () => void,
        lap: () => void
    };
    buttons: {
        toggle: any,
        start: any,
        stop: any,
        reset: any,
        lap: any,
        resetLap: any,
        hide: any
    };

    start(): void;
    stop(): void;
    toggle(): void;
    reset(): void;
    lap(): void;
    resetLap(): void;
    getTime(): number;
    setTime(t: number): void;
    getEllapsedTime(): number;
    setEllapsedTime(t: number, change?: boolean): void;
}

declare interface MbscTimespanOptions extends MbscScrollerOptions {
    // Settings
    defaultValue?: number;
    max?: number;
    min?: number;
    steps?: Array<number>;
    useShortLabels?: boolean;
    wheelOrder?: string;
    labels?: Array<string>;
    labelsShort?: Array<string>;
}

declare class Timespan extends Scroller<MbscTimespanOptions> {
    getVal(temp?: boolean, formatted?: boolean): number;
}

declare interface MbscTreelistOptions extends MbscScrollerOptions {
    defaultValue?: Array<number>;
    inputClass?: string;
    invalid?: Array<any>;
    labels?: Array<string>;
    placeholder?: string;
    showInput?: boolean;
}

declare class Treelist extends List<MbscTreelistOptions> { }

declare interface MbscPopupOptions extends MbscFrameOptions {
    okText?: string;
    onSet?(event: { valueText?: string }, inst: any): void;
}

declare interface MbscWidgetOptions extends MbscPopupOptions { }

declare class Popup extends Frame<MbscPopupOptions> {
}

declare class Widget extends Popup {
    settings: MbscWidgetOptions;
    constructor(element: any, settings: MbscWidgetOptions);
}

declare interface MbscWidgetOptions extends MbscPopupOptions { }
 
declare interface IMobiscroll extends _IMobiscroll {
    card(selector: string | HTMLElement, options?: MbscCardOptions): Card;
    calendar(selector: string | HTMLElement, options?: MbscCalendarOptions): Calendar;
    color(selector: string | HTMLElement, options?: MbscColorOptions): Color;
    datetime(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    date(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    time(selector: string | HTMLElement, options?: MbscDatetimeOptions): DateTime;
    eventcalendar(selector: string | HTMLElement, options?: MbscEventcalendarOptions): Eventcalendar;
    form(selector: string | HTMLElement, options?: MbscFormOptions): Form;
    page(selector: string | HTMLElement, options?: MbscPageOptions): Page;
    image(selector: string | HTMLElement, options?: MbscImageOptions): ImageScroller;
    listview(selector: string | HTMLElement, options?: MbscListviewOptions): ListView;
    optionlist(selector: string | HTMLElement, options?: MbscOptionlistOptions): Optionlist;
    measurement(selector: string | HTMLElement, options?: MbscMeasurementOptions): Measurement;
    mass(selector: string | HTMLElement, options?: MbscMassOptions): Mass;
    distance(selector: string | HTMLElement, options?: MbscDistanceOptions): Distance;
    force(selector: string | HTMLElement, options?: MbscForceOptions): Force;
    speed(selector: string | HTMLElement, options?: MbscSpeedOptions): Speed;
    temperature(selector: string | HTMLElement, options?: MbscTemperatureOptions): Temperature;
    nav(selector: string | HTMLElement, options?: MbscNavOptions): Navigation;
    number(selector: string | HTMLElement, options?: MbscNumberOptions): NumberScroller;
    numpad(selector: string | HTMLElement, options?: MbscNumpadOptions): Numpad;
    range(selector: string | HTMLElement, options?: MbscRangeOptions): RangePicker;
    scroller(selector: string | HTMLElement, options?: MbscScrollerOptions): Scroller;
    scrollview(selector: string | HTMLElement, options?: MbscScrollViewOptions): ScrollView;
    select(selector: string | HTMLElement, options?: MbscSelectOptions): Select;
    timer(selector: string | HTMLElement, options?: MbscTimerOptions): Timer;
    timespan(selector: string | HTMLElement, options?: MbscTimespanOptions): Timespan;
    toast(options?: any): any;
    treelist(selector: string | HTMLElement, options?: MbscTreelistOptions): Treelist;
    popup(selector: string | HTMLElement, options?: MbscPopupOptions): Popup;
    widget(selector: string | HTMLElement, options?: MbscWidgetOptions): Widget;
    instances: string[];
}

declare const mobiscroll: IMobiscroll;