export type ErrorResponse = {
    message?: string;
    errors?: {
        [key: string]: string[];
    };
};
