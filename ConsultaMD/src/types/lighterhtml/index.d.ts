type TemplateFunction<T> = (template: TemplateStringsArray, ...values: any[]) => T;

declare interface Tag<T> extends TemplateFunction<any> {
  for: (object: object, id?: string) => Tag<T>;
}

declare const html: Tag<HTMLElement>;
declare const svg: Tag<SVGElement>;
declare function render(node: HTMLElement, renderer: () => any): any;
declare function hook(hook: Function) : {
  html: Tag<HTMLElement>
  svg: Tag<SVGElement>
};
