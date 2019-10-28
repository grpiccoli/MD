declare interface ResultsVM {
    place: Place;
    items: Array<ResultVM>;
}

declare interface Place {
    address: string;
    cId: string;
    name: string;
    latitude: number;
    longitude: number;
    photoId: string;
    id: string;
}

declare interface TimeSlotVM {
    id: number;
    startTime: Date;
    hora: string;
}

declare interface TimeSlotsVM {
    id: number;
    startTime: string;
}

declare interface ResultVM {
    run: number;
    price: number;
    dr: string;
    especialidad: string;
    office: string;
    experience: number;
    sex: boolean;
    insurances: Array<Insurance>;
    nextTS: TimeSlotVM;
    cardId: number;
    match: boolean;
}

declare enum Insurance {
    "Particular" = 0,
    "Fonasa" = 1,
    "Banmédica" = 2,
    "Colmena" = 3,
    "Consalud" = 4,
    "CruzBlanca" = 5,
    "Nueva Masvida" = 6,
    "Vida Tres" = 7
}