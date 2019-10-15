//To Title case function
export function toTitleCase(str: string) {
    return str.replace(/(?:^|\s)\w/g, function (match) {
        return match.toUpperCase();
    });
}