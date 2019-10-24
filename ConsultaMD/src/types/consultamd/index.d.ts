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
    next: Date;
    cardId: number;
    hora: string;
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