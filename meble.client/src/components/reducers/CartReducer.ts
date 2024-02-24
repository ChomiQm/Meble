import {
    ADD_TO_CART,
    REMOVE_FROM_CART,
    CLEAR_CART,
    INCREASE_QUANTITY,
    DECREASE_QUANTITY,
    CartItem
} from '../actions/CartAction';

type IncreaseQuantityAction = { type: typeof INCREASE_QUANTITY; payload: { id: number, maxQuantity: number }; };
type DecreaseQuantityAction = { type: typeof DECREASE_QUANTITY; payload: number; };
type AddToCartAction = { type: typeof ADD_TO_CART; payload: CartItem; };
type RemoveFromCartAction = { type: typeof REMOVE_FROM_CART; payload: number; };
type ClearCartAction = { type: typeof CLEAR_CART; };

type CartAction = IncreaseQuantityAction | DecreaseQuantityAction | AddToCartAction | RemoveFromCartAction | ClearCartAction;

export interface CartState { cartItems: CartItem[]; }

const initialState: CartState = { cartItems: JSON.parse(window.localStorage.getItem('cart') || '[]'), };

const cartReducer = (state = initialState, action: CartAction): CartState => {
    switch (action.type) {
        case ADD_TO_CART: {
            const existingCartItem = state.cartItems.find(item => item.id === action.payload.id);
            if (existingCartItem && existingCartItem.quantity < existingCartItem.maxQuantity) {
                return {
                    ...state,
                    cartItems: state.cartItems.map(item =>
                        item.id === action.payload.id ? { ...item, quantity: item.quantity + 1 } : item
                    ),
                };
            } else if (!existingCartItem) {
                return {
                    ...state,
                    cartItems: [...state.cartItems, { ...action.payload, quantity: 1 }],
                };
            }
            return state;
        }
        case REMOVE_FROM_CART: {
            return {
                ...state,
                cartItems: state.cartItems.filter(item => item.id !== action.payload),
            };
        }
        case INCREASE_QUANTITY: {
            const item = state.cartItems.find(item => item.id === action.payload.id);
            if (item && item.quantity < action.payload.maxQuantity) {
                return {
                    ...state,
                    cartItems: state.cartItems.map(item =>
                        item.id === action.payload.id ? { ...item, quantity: item.quantity + 1 } : item
                    ),
                };
            }
            return state;
        }
        case DECREASE_QUANTITY: {
            return {
                ...state,
                cartItems: state.cartItems.map(item =>
                    item.id === action.payload && item.quantity > 1 ? { ...item, quantity: item.quantity - 1 } : item
                ),
            };
        }
        case CLEAR_CART: {
            return { ...state, cartItems: [] };
        }
        default:
            return state;
    }
};

export default cartReducer;
