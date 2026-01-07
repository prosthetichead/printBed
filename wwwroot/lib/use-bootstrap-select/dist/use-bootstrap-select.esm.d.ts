export default class UseBootstrapSelect {
    selectElement: HTMLSelectElement;
    private static instances;
    private configObject?;
    constructor(selectElement: HTMLSelectElement, config?: OptionalType<Config>);
    private getConfigFromGlobal;
    private getConfigFromAttributes;
    private getConfig;
    private setSelected;
    getValue(): string | string[] | null;
    setValue(value: Value): void;
    addValue(value: Value): void;
    removeValue(value: Value): void;
    clearValue(): void;
    addOption(value: string, text?: string, selected?: boolean, position?: Config['createPosition']): void;
    update(): void;
    show(): void;
    hide(): void;
    toggle(): void;
    private init;
    private getClassList;
    private getItems;
    private getSelected;
    private render;
    static getOrCreateInstance(selectElement: HTMLSelectElement): UseBootstrapSelect;
    static clearAll(scope?: HTMLElement | null): void;
}
type Value = string | string[];
export interface Config {
    position: 'up' | 'down';
    maxHeight: string;
    clearable: boolean;
    searchable: boolean;
    noResultsText: string;
    creatable: boolean;
    creatableText: string;
    createPosition: 'first' | 'last';
}
export type OptionalType<T> = {
    [P in keyof T]?: T[P];
};
export {};
