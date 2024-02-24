export const translateStatus = (status: string) => {
    const statusTranslations: { [key: string]: string } = {
        'DELIVERED': 'Dostarczone',
        'PENDING': 'Oczekujące',
    };

    return statusTranslations[status] || status;
};
