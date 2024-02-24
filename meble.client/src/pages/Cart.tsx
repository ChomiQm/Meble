import { useSelector, useDispatch } from 'react-redux';
import { removeFromCart, clearCart, increaseQuantity, decreaseQuantity } from '../components/actions/CartAction';
import { RootState } from '../../store';
import useLocalStorage from '../hooks/useLocalStorage';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import AuthContext from '../services/AuthContext';
import { useContext } from 'react';

const Cart = () => {
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const dispatch = useDispatch();
    const cartItems = useSelector((state: RootState) => state.cart.cartItems);
    const [, setCartItemsLocalStorage] = useLocalStorage('cart', []);
    useLocalStorage('cart', cartItems);

    const handleRemoveFromCart = (id: number) => {
        dispatch(removeFromCart(id));
    };

    const handleIncreaseQuantity = (id: number, maxQuantity: number) => {
        const item = cartItems.find(item => item.id === id);
        if (item && item.quantity < maxQuantity) {
            dispatch(increaseQuantity(id, maxQuantity));
        }
    };

    const handlePlaceOrder = async () => {
        const furnitureIds = cartItems.map(item => item.id,);
        const QuantityOrdered = cartItems.map(item => item.quantity);
        const totalOrderValue = calculateTotal();
        const totalItemsOrdered = cartItems.reduce((total, item) => total + item.quantity, 0);

        try {
            const response = await fetchWithAuth('https://localhost:7197/order/createOrder', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`,
                },
                body: JSON.stringify({
                    furnitureIds,
                    totalOrderValue,
                    totalItemsOrdered,
                    QuantityOrdered
                }),
            }, refreshAccessToken, logout);
        
            if (response.ok) {
                for (const item of cartItems) {
                    try {
                        const furnitureResponse = await fetchWithAuth(`https://localhost:7197/furnitures/getFurniture/${item.id}`, {
                            method: 'GET',
                            headers: {
                                'Authorization': `Bearer ${accessToken}`,
                            },
                        }, refreshAccessToken, logout);

                        if (furnitureResponse.ok) {
                            const furniture = await furnitureResponse.json();
                            const newQuantity = furniture.quantity - item.quantity;

                            await fetchWithAuth(`https://localhost:7197/furnitures/updateFurniture/${item.id}`, {
                                method: 'PUT',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Authorization': `Bearer ${accessToken}`,
                                },
                                body: JSON.stringify({ ...furniture, quantity: newQuantity })
                            }, refreshAccessToken, logout);
                        }
                    } catch (error) {
                        console.error(`Błąd aktualizacji mebla o ID: ${item.id}`, error);
                    }
                }

                alert("Zamówienie zostało złożone.");
                dispatch(clearCart());
                setCartItemsLocalStorage([]);
            } else {
                alert("Wystąpił problem podczas składania zamówienia.");
            }
        } catch (error) {
            console.error("Error placing order:", error);
            alert("Wystąpił błąd podczas składania zamówienia.");
        }
    };

    const handleDecreaseQuantity = (id: number) => {
        dispatch(decreaseQuantity(id));
    };

    const handleClearCart = () => {
        dispatch(clearCart());
        setCartItemsLocalStorage([]);
    };

    const calculateTotal = () => {
        return cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
    };

    return (
        <div className='h-screen'>
        <div className='flex flex-col  justify-center items-center gap-10 text-white '>
            <h2 className='text-6xl my-10 containerCustom font-bold font-size hidden  md:block'>Koszyk</h2>
            <div className='flex flex-col w-5/6 min-h-10 justify-center items-center glass h-full p-10'>
               
                {cartItems.length === 0 ? (
                    <p className='text-xl'>Koszyk jest pusty</p>
                ) : (
                    <>
                        <button className='my-10' onClick={handleClearCart}>Wyczyść koszyk</button>
                        <ul className='flex gap-10 flex-wrap justify-center  '>
                            {cartItems.map((item) => (
                                <li className='glass rounded max-w-[300px] grow my-10 transform transition-transform duration-500 hover:scale-105 glass p-4  flex flex-col justify-center items-center text-center gap-5' key={item.id}>
                                    <img className='w-full rounded-2xl' src={item.photos?.[0]?.url} alt={item.photos?.[0]?.description} />
                                    <span className='text-black'>{item.name}</span>
                                    <span className='text-black'>{item.price.toFixed(2)} zł</span>
                                    <span className='text-black'>Ilość: {item.quantity}</span>
                                    <button onClick={() => handleIncreaseQuantity(item.id, item.maxQuantity)}>+</button>
                                    <button onClick={() => handleDecreaseQuantity(item.id)}>-</button>
                                    <button className='px-4 my-1 py-2 flex justify-center items-center rounded-xl  brown transition-transform duration-500 hover:scale-105' onClick={() => handleRemoveFromCart(item.id)}>Usuń</button>
                                </li>
                            ))}
                        </ul>
                        <p className='text-2xl text-white'>Całkowita kwota: {calculateTotal().toFixed(2)} zł</p>
                        <button className='px-4 my-1 py-2 flex justify-center items-center rounded-xl  brown transition-transform duration-500 hover:scale-105' onClick={handlePlaceOrder}>Złóż zamówienie</button> {/* Dodany przycisk do składania zamówienia */}
                    </>
                )}
            </div>
        </div>
        </div>
    );
}
export default Cart;
