export const ADD_TO_CART = 'ADD_TO_CART';
export const REMOVE_FROM_CART = 'REMOVE_FROM_CART';
export const CLEAR_CART = 'CLEAR_CART';
export const INCREASE_QUANTITY = 'INCREASE_QUANTITY';
export const DECREASE_QUANTITY = 'DECREASE_QUANTITY';

export interface CartItem {
    id: number;
    name: string;
    price: number;
    description?: string;
    isAvailable: boolean;
    quantity: number;
    maxQuantity: number;
    photos?: Array<{ url: string; description?: string; }>;
    categories?: Array<{ name: string; }>;
}

// Action creators
export const addToCart = (item: CartItem) => ({
    type: ADD_TO_CART,
    payload: item,
});

export const removeFromCart = (id: number) => ({
    type: REMOVE_FROM_CART,
    payload: id,
});

export const clearCart = () => ({
    type: CLEAR_CART,
});

export const increaseQuantity = (id: number, maxQuantity: number) => ({
    type: INCREASE_QUANTITY,
    payload: { id, maxQuantity },
});

export const decreaseQuantity = (id: number) => ({
    type: DECREASE_QUANTITY,
    payload: id,
});
