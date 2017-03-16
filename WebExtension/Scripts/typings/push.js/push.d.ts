interface PushNotificationDetails {
    body: string;
    icon?: string;
    timeout: number;
    onClick(): void;
}

interface PushStatic {
    (el: Element): PushInstance;
    new (el: Element): PushInstance;
    create(title: string, details: PushNotificationDetails): void;
}

interface PushInstance {
    create(title: string, details: PushNotificationDetails): void;
}

declare var Push: PushStatic;

declare module "push" {
    export = Push;
}
