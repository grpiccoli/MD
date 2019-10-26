var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Injectable } from '@angular/core';
import { IonicNativePlugin, cordova } from '@ionic-native/core';
var WheelSelector = /** @class */ (function (_super) {
    __extends(WheelSelector, _super);
    function WheelSelector() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    WheelSelector.prototype.show = function (options) { return cordova(this, "show", {}, arguments); };
    WheelSelector.prototype.hideSelector = function () { return cordova(this, "hideSelector", { "platforms": ["iOS"] }, arguments); };
    WheelSelector.pluginName = "WheelSelector";
    WheelSelector.plugin = "cordova-wheel-selector-plugin";
    WheelSelector.pluginRef = "SelectorCordovaPlugin";
    WheelSelector.repo = "https://github.com/jasonmamy/cordova-wheel-selector-plugin";
    WheelSelector.platforms = ["Android", "iOS"];
    WheelSelector = __decorate([
        Injectable()
    ], WheelSelector);
    return WheelSelector;
}(IonicNativePlugin));
export { WheelSelector };
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi8uLi8uLi8uLi8uLi9zcmMvQGlvbmljLW5hdGl2ZS9wbHVnaW5zL3doZWVsLXNlbGVjdG9yL25neC9pbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBQUEsT0FBTyxFQUFFLFVBQVUsRUFBRSxNQUFNLGVBQWUsQ0FBQztBQUMzQyxPQUFPLDhCQUFzQyxNQUFNLG9CQUFvQixDQUFDOztJQXNMckMsaUNBQWlCOzs7O0lBUWxELDRCQUFJLGFBQUMsT0FBNkI7SUFXbEMsb0NBQVk7Ozs7OztJQW5CRCxhQUFhO1FBRHpCLFVBQVUsRUFBRTtPQUNBLGFBQWE7d0JBdkwxQjtFQXVMbUMsaUJBQWlCO1NBQXZDLGFBQWEiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBJbmplY3RhYmxlIH0gZnJvbSAnQGFuZ3VsYXIvY29yZSc7XG5pbXBvcnQgeyBDb3Jkb3ZhLCBJb25pY05hdGl2ZVBsdWdpbiwgUGx1Z2luIH0gZnJvbSAnQGlvbmljLW5hdGl2ZS9jb3JlJztcblxuZXhwb3J0IGludGVyZmFjZSBXaGVlbFNlbGVjdG9ySXRlbSB7XG4gIGRlc2NyaXB0aW9uPzogc3RyaW5nO1xufVxuXG5leHBvcnQgaW50ZXJmYWNlIERlZmF1bHRJdGVtIHtcbiAgaW5kZXg6IG51bWJlcjtcbiAgdmFsdWU6IHN0cmluZztcbn1cblxuZXhwb3J0IGludGVyZmFjZSBXaGVlbFNlbGVjdG9yT3B0aW9ucyB7XG4gIC8qKlxuICAgKiBUaGUgdGl0bGUgb2YgdGhlIHNlbGVjdG9yJ3MgaW5wdXQgYm94XG4gICAqL1xuICB0aXRsZTogc3RyaW5nO1xuXG4gIC8qKlxuICAgKiBUaGUgaXRlbXMgdG8gZGlzcGxheSAoYXJyYXkgb2YgaXRlbXMpLlxuICAgKi9cbiAgaXRlbXM6IFdoZWVsU2VsZWN0b3JJdGVtW11bXTtcblxuICAvKipcbiAgICogV2hpY2ggaXRlbXMgdG8gZGlzcGxheSBieSBkZWZhdWx0LlxuICAgKi9cbiAgZGVmYXVsdEl0ZW1zPzogRGVmYXVsdEl0ZW1bXTtcblxuICAvKipcbiAgICogVGhlICdvaycgYnV0dG9uIHRleHRcbiAgICogRGVmYXVsdDogRG9uZVxuICAgKi9cbiAgcG9zaXRpdmVCdXR0b25UZXh0Pzogc3RyaW5nO1xuXG4gIC8qKlxuICAgKiBUaGUgJ2NhbmNlbCcgYnV0dG9uIHRleHRcbiAgICogRGVmYXVsdDogQ2FuY2VsXG4gICAqL1xuICBuZWdhdGl2ZUJ1dHRvblRleHQ/OiBzdHJpbmc7XG5cbiAgLyoqXG4gICAqIEFuZHJvaWQgb25seSAtIHRoZW1lIGNvbG9yLCAnbGlnaHQnIG9yICdkYXJrJy5cbiAgICogRGVmYXVsdDogbGlnaHRcbiAgICovXG4gIHRoZW1lPzogJ2xpZ2h0JyB8ICdkYXJrJztcblxuICAvKipcbiAgICogV2hldGhlciB0byBoYXZlIHRoZSB3aGVlbHMgJ3dyYXAnIChBbmRyb2lkIG9ubHkpXG4gICAqIERlZmF1bHQ6IGZhbHNlXG4gICAqL1xuICB3cmFwV2hlZWxUZXh0PzogYm9vbGVhbjtcblxuICAvKipcbiAgICogVGhlIGpzb24ga2V5IHRvIGRpc3BsYXksIGJ5IGRlZmF1bHQgaXQgaXMgZGVzY3JpcHRpb24sIHRoaXMgYWxsb3dzIGZvciBzZXR0aW5nIGFueVxuICAgKiBrZXkvdmFsdWUgdG8gYmUgZGlzcGxheWVkXG4gICAqIERlZmF1bHQ6IGRlc2NyaXB0aW9uXG4gICAqL1xuICBkaXNwbGF5S2V5Pzogc3RyaW5nO1xufVxuXG5leHBvcnQgaW50ZXJmYWNlIFdoZWVsU2VsZWN0b3JEYXRhIHtcbiAgZGF0YTogYW55O1xufVxuXG4vKipcbiAqIEBiZXRhXG4gKiBAbmFtZSBXaGVlbFNlbGVjdG9yIFBsdWdpblxuICogQGRlc2NyaXB0aW9uIE5hdGl2ZSB3aGVlbCBzZWxlY3RvciBmb3IgQ29yZG92YSAoQW5kcm9pZC9pT1MpLlxuICpcbiAqIEB1c2FnZVxuICogYGBgXG4gKiBpbXBvcnQgeyBXaGVlbFNlbGVjdG9yIH0gZnJvbSAnQGlvbmljLW5hdGl2ZS93aGVlbC1zZWxlY3Rvci9uZ3gnO1xuICpcbiAqXG4gKiBjb25zdHJ1Y3Rvcihwcml2YXRlIHNlbGVjdG9yOiBXaGVlbFNlbGVjdG9yKSB7IH1cbiAqXG4gKiAuLi5cbiAqXG4gKiBjb25zdCBqc29uRGF0YSA9IHtcbiAqICAgbnVtYmVyczogW1xuICogICAgeyBkZXNjcmlwdGlvbjogXCIxXCIgfSxcbiAqICAgICB7IGRlc2NyaXB0aW9uOiBcIjJcIiB9LFxuICogICAgIHsgZGVzY3JpcHRpb246IFwiM1wiIH1cbiAqICAgXSxcbiAqICAgZnJ1aXRzOiBbXG4gKiAgICAgeyBkZXNjcmlwdGlvbjogXCJBcHBsZVwiIH0sXG4gKiAgICAgeyBkZXNjcmlwdGlvbjogXCJCYW5hbmFcIiB9LFxuICogICAgIHsgZGVzY3JpcHRpb246IFwiVGFuZ2VyaW5lXCIgfVxuICogICBdLFxuICogICBmaXJzdE5hbWVzOiBbXG4gKiAgICAgeyBuYW1lOiBcIkZyZWRcIiwgaWQ6ICcxJyB9LFxuICogICAgIHsgbmFtZTogXCJKYW5lXCIsIGlkOiAnMicgfSxcbiAqICAgICB7IG5hbWU6IFwiQm9iXCIsIGlkOiAnMycgfSxcbiAqICAgICB7IG5hbWU6IFwiRWFybFwiLCBpZDogJzQnIH0sXG4gKiAgICAgeyBuYW1lOiBcIkV1bmljZVwiLCBpZDogJzUnIH1cbiAqICAgXSxcbiAqICAgbGFzdE5hbWVzOiBbXG4gKiAgICAgeyBuYW1lOiBcIkpvaG5zb25cIiwgaWQ6ICcxMDAnIH0sXG4gKiAgICAgeyBuYW1lOiBcIkRvZVwiLCBpZDogJzEwMScgfSxcbiAqICAgICB7IG5hbWU6IFwiS2luaXNoaXdhXCIsIGlkOiAnMTAyJyB9LFxuICogICAgIHsgbmFtZTogXCJHb3Jkb25cIiwgaWQ6ICcxMDMnIH0sXG4gKiAgICAgeyBuYW1lOiBcIlNtaXRoXCIsIGlkOiAnMTA0JyB9XG4gKiAgIF1cbiAqIH1cbiAqXG4gKiAuLi5cbiAqXG4gKiAvLyBiYXNpYyBudW1iZXIgc2VsZWN0aW9uLCBpbmRleCBpcyBhbHdheXMgcmV0dXJuZWQgaW4gdGhlIHJlc3VsdFxuICogIHNlbGVjdEFOdW1iZXIoKSB7XG4gKiAgICB0aGlzLnNlbGVjdG9yLnNob3coe1xuICogICAgICB0aXRsZTogXCJIb3cgTWFueT9cIixcbiAqICAgICAgaXRlbXM6IFtcbiAqICAgICAgICB0aGlzLmpzb25EYXRhLm51bWJlcnNcbiAqICAgICAgXSxcbiAqICAgIH0pLnRoZW4oXG4gKiAgICAgIHJlc3VsdCA9PiB7XG4gKiAgICAgICAgY29uc29sZS5sb2cocmVzdWx0WzBdLmRlc2NyaXB0aW9uICsgJyBhdCBpbmRleDogJyArIHJlc3VsdFswXS5pbmRleCk7XG4gKiAgICAgIH0sXG4gKiAgICAgIGVyciA9PiBjb25zb2xlLmxvZygnRXJyb3I6ICcsIGVycilcbiAqICAgICAgKTtcbiAqICB9XG4gKlxuICogIC4uLlxuICpcbiAqICAvLyBiYXNpYyBzZWxlY3Rpb24sIHNldHRpbmcgaW5pdGlhbCBkaXNwbGF5ZWQgZGVmYXVsdCB2YWx1ZXM6ICczJyAnQmFuYW5hJ1xuICogIHNlbGVjdEZydWl0KCkge1xuICogICAgdGhpcy5zZWxlY3Rvci5zaG93KHtcbiAqICAgICAgdGl0bGU6IFwiSG93IE11Y2g/XCIsXG4gKiAgICAgIGl0ZW1zOiBbXG4gKiAgICAgICAgdGhpcy5qc29uRGF0YS5udW1iZXJzLCB0aGlzLmpzb25EYXRhLmZydWl0c1xuICogICAgICBdLFxuICogICAgICBwb3NpdGl2ZUJ1dHRvblRleHQ6IFwiT2tcIixcbiAqICAgICAgbmVnYXRpdmVCdXR0b25UZXh0OiBcIk5vcGVcIixcbiAqICAgICAgZGVmYXVsdEl0ZW1zOiBbXG4gKiAgXHQgIHtpbmRleDowLCB2YWx1ZTogdGhpcy5qc29uRGF0YS5udW1iZXJzWzJdLmRlc2NyaXB0aW9ufSxcbiAqICBcdCAge2luZGV4OiAxLCB2YWx1ZTogdGhpcy5qc29uRGF0YS5mcnVpdHNbM10uZGVzY3JpcHRpb259XG4gKiAgXHRdXG4gKiAgICB9KS50aGVuKFxuICogICAgICByZXN1bHQgPT4ge1xuICogICAgICAgIGNvbnNvbGUubG9nKHJlc3VsdFswXS5kZXNjcmlwdGlvbiArICcgJyArIHJlc3VsdFsxXS5kZXNjcmlwdGlvbik7XG4gKiAgICAgIH0sXG4gKiAgICAgIGVyciA9PiBjb25zb2xlLmxvZygnRXJyb3I6ICcgKyBKU09OLnN0cmluZ2lmeShlcnIpKVxuICogICAgICApO1xuICogIH1cbiAqXG4gKiAgLi4uXG4gKlxuICogIC8vIG1vcmUgY29tcGxleCBhcyBvdmVycmlkZXMgd2hpY2gga2V5IHRvIGRpc3BsYXlcbiAqICAvLyB0aGVuIHJldHJpZXZlIHByb3BlcnRpZXMgZnJvbSBvcmlnaW5hbCBkYXRhXG4gKiAgc2VsZWN0TmFtZXNVc2luZ0Rpc3BsYXlLZXkoKSB7XG4gKiAgICB0aGlzLnNlbGVjdG9yLnNob3coe1xuICogICAgICB0aXRsZTogXCJXaG8/XCIsXG4gKiAgICAgIGl0ZW1zOiBbXG4gKiAgICAgICAgdGhpcy5qc29uRGF0YS5maXJzdE5hbWVzLCB0aGlzLmpzb25EYXRhLmxhc3ROYW1lc1xuICogICAgICBdLFxuICogICAgICBkaXNwbGF5S2V5OiAnbmFtZScsXG4gKiAgICAgIGRlZmF1bHRJdGVtczogW1xuICogIFx0ICB7aW5kZXg6MCwgdmFsdWU6IHRoaXMuanNvbkRhdGEuZmlyc3ROYW1lc1syXS5uYW1lfSxcbiAqICAgICAgICB7aW5kZXg6IDAsIHZhbHVlOiB0aGlzLmpzb25EYXRhLmxhc3ROYW1lc1szXS5uYW1lfVxuICogICAgICBdXG4gKiAgICB9KS50aGVuKFxuICogICAgICByZXN1bHQgPT4ge1xuICogICAgICAgIGNvbnNvbGUubG9nKHJlc3VsdFswXS5uYW1lICsgJyAoaWQ9ICcgKyB0aGlzLmpzb25EYXRhLmZpcnN0TmFtZXNbcmVzdWx0WzBdLmluZGV4XS5pZCArICcpLCAnICtcbiAqICAgICAgICAgIHJlc3VsdFsxXS5uYW1lICsgJyAoaWQ9JyArIHRoaXMuanNvbkRhdGEubGFzdE5hbWVzW3Jlc3VsdFsxXS5pbmRleF0uaWQgKyAnKScpO1xuICogICAgICB9LFxuICogICAgICBlcnIgPT4gY29uc29sZS5sb2coJ0Vycm9yOiAnICsgSlNPTi5zdHJpbmdpZnkoZXJyKSlcbiAqICAgICAgKTtcbiAqICB9XG4gKlxuICogYGBgXG4gKlxuICogQGludGVyZmFjZXNcbiAqIFdoZWVsU2VsZWN0b3JPcHRpb25zXG4gKi9cbkBQbHVnaW4oe1xuICBwbHVnaW5OYW1lOiAnV2hlZWxTZWxlY3RvcicsXG4gIHBsdWdpbjogJ2NvcmRvdmEtd2hlZWwtc2VsZWN0b3ItcGx1Z2luJyxcbiAgcGx1Z2luUmVmOiAnU2VsZWN0b3JDb3Jkb3ZhUGx1Z2luJyxcbiAgcmVwbzogJ2h0dHBzOi8vZ2l0aHViLmNvbS9qYXNvbm1hbXkvY29yZG92YS13aGVlbC1zZWxlY3Rvci1wbHVnaW4nLFxuICBwbGF0Zm9ybXM6IFsnQW5kcm9pZCcsICdpT1MnXVxufSlcblxuQEluamVjdGFibGUoKVxuZXhwb3J0IGNsYXNzIFdoZWVsU2VsZWN0b3IgZXh0ZW5kcyBJb25pY05hdGl2ZVBsdWdpbiB7XG5cbiAgLyoqXG4gICAqIFNob3dzIHRoZSB3aGVlbCBzZWxlY3RvclxuICAgKiBAcGFyYW0ge1doZWVsU2VsZWN0b3JPcHRpb25zfSBvcHRpb25zIE9wdGlvbnMgZm9yIHRoZSB3aGVlbCBzZWxlY3RvclxuICAgKiBAcmV0dXJucyB7UHJvbWlzZTxXaGVlbFNlbGVjdG9yRGF0YT59IFJldHVybnMgYSBwcm9taXNlIHRoYXQgcmVzb2x2ZXMgd2l0aCB0aGUgc2VsZWN0ZWQgaXRlbXMsIG9yIGFuIGVycm9yLlxuICAgKi9cbiAgQENvcmRvdmEoKVxuICBzaG93KG9wdGlvbnM6IFdoZWVsU2VsZWN0b3JPcHRpb25zKTogUHJvbWlzZTxXaGVlbFNlbGVjdG9yRGF0YT4ge1xuICAgIHJldHVybjtcbiAgfVxuXG4gIC8qKlxuICAgKiBIaWRlIHRoZSBzZWxlY3RvclxuICAgKiBAcmV0dXJucyB7UHJvbWlzZTx2b2lkPn1cbiAgICovXG4gIEBDb3Jkb3ZhKHtcbiAgICBwbGF0Zm9ybXM6IFsnaU9TJ11cbiAgfSlcbiAgaGlkZVNlbGVjdG9yKCk6IFByb21pc2U8dm9pZD4ge1xuICAgIHJldHVybjtcbiAgfVxufVxuIl19