export class RootFilter {

    // order : Order | undefined;
    filters: Filter[] = [];
    valueFilters: Filter[] = [];

    logic : string | undefined;
    sortingParams : SortingParams[] = [];
    valueSortingParams : SortingParams[] = [];

    take : number = 0;
    skip : number  = 0;
    valueTake : number = 0;
    valueSkip : number  = 0;

}

export class Order {
    // field : string | undefined;
    // operator : string | undefined;

    // value : any | undefined;
}

export class Filter {
    field : string | undefined;
    operator : string | undefined;
    value : any | undefined;
}

export class SortingParams {
    field : string | undefined;
    order : number = -1;
    type : string | undefined;
}
