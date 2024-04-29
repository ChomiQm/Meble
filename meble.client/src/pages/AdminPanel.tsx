import { useState, useEffect, useContext } from 'react';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';
import { OrderDetails } from './Order';
import { translateStatus } from '../components/utils/translateStatus';

interface ExtendedOrderDetails extends OrderDetails {
    userFirstName: string;
    userSurname: string;
    userCountry: string;
    userTown: string;
    userStreet?: string;
    userHomeNumber?: number;
    userFlatNumber?: string;
}


const AdminPanel = () => {
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [error, setError] = useState('');
    const [orders, setOrders] = useState<ExtendedOrderDetails[]>([]);

    useEffect(() => {
        const fetchAllUsersOrders = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/order/getAllUserOrders', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);
                if (response.ok) {
                    const data = await response.json();
                    setOrders(data);
                } else {
                    setError('Błąd podczas pobierania zamówień użytkowników');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas pobierania zamówień użytkowników');
                logout();
            }
        };

        fetchAllUsersOrders();
    }, [accessToken, refreshAccessToken, logout]);

    const toggleOrderStatus = async (order: OrderDetails) => {
        const newStatus = order.orderStatus === "PENDING" ? "DELIVERED" : "PENDING";

        try {
            const response = await fetchWithAuth(`https://localhost:7197/order/updateOrderStatus/${order.orderId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ OrderStatus: newStatus }),
            }, refreshAccessToken, logout);

            if (response.ok) {
                setOrders(orders.map(o => o.orderId === order.orderId ? { ...o, orderStatus: newStatus } : o));
            } else {
                setError('Błąd podczas aktualizacji statusu zamówienia');
            }
        } catch (error) {
            setError('Wystąpił błąd podczas aktualizacji statusu zamówienia');
            logout();
        }
    };

    const deleteOrder = async (orderId: number) => {
        try {
            const response = await fetchWithAuth(`https://localhost:7197/order/deleteOrder/${orderId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                },
            }, refreshAccessToken, logout);

            if (response.ok) {
                setOrders(orders.filter(order => order.orderId !== orderId));
            } else {
                setError('Błąd podczas usuwania zamówienia');
            }
        } catch (error) {
            setError('Wystąpił błąd podczas usuwania zamówienia');
            logout();
        }
    };

    if (!accessToken) {
        return <div>Musisz być zalogowany, aby zobaczyć panel admina.</div>;
    }
    
    return (
        <div className='flex flex-col items-center w-full p-10'>
            {error && <div className='text-red-500'>{error}</div>}
            <div className='w-full max-w-4xl'>
                <h1 className='text-3xl font-bold text-center mb-6 text-white'>Panel Admina - Wszystkie Zamówienia</h1>
                <div className='max-h-[600px] overflow-auto rounded-lg shadow bg-white'>
                    <div className='p-5 space-y-4'>
                        {orders.length > 0 ? (
                            orders.map((order) => (
                                <div key={order.orderId} className='relative border-b last:border-b-0 p-4'>
                                    <div className='flex flex-col md:flex-row justify-between md:items-start'>
                                        <div className='flex-1'>
                                            <h2 className='text-lg font-semibold'>Zamówienie #{order.orderId}</h2>
                                            <p>Data zamówienia: {order.orderDateOfOrder ?? 'Brak daty'}</p>
                                            <p>Całkowita wartość: {order.totalOrderValue} zł</p>
                                            <p>Ilość pozycji: {order.totalItemsOrdered}</p>
                                            <p>Status zamówienia: {translateStatus(order.orderStatus)}</p>
                                            {order.furnitures.map((furniture, index) => (
                                            <p key={furniture.furnitureId} className='text-sm'>
                                                {furniture.furnitureName} - {furniture.furniturePrice} zł - {order.quantityOrdered![index]} sztuk
                                            </p>
                                        ))}
                                        </div>
                                        <div className='flex-1 mt-4 md:mt-0 md:ml-4'>
                                            <h2 className='text-lg font-semibold'>Dane Użytkownika</h2>
                                            <p>Imię: {order.userFirstName}</p>
                                            <p>Nazwisko: {order.userSurname}</p>
                                            <p>Kraj: {order.userCountry}</p>
                                            <p>Miasto: {order.userTown}</p>
                                            <p>Ulica: {order.userStreet ?? 'Brak'}</p>
                                            <p>{order.userHomeNumber && !order.userFlatNumber ? `Numer domu: ${order.userHomeNumber}` : ''}</p>
                                            <p>{order.userFlatNumber ? `Numer mieszkania: ${order.userFlatNumber}` : ''}</p>
                                        </div>
                                    </div>
                                    {/* Przyciski akcji */}
                                    <div className='flex flex-col md:flex-row md:justify-end space-y-2 md:space-y-0 md:space-x-2 mt-4'>
                                        <button
                                            onClick={() => deleteOrder(order.orderId)}
                                            className='bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded transition ease-in-out duration-150 focus:outline-none focus:ring-2 focus:ring-red-200 focus:ring-opacity-50 w-full md:w-auto'
                                        >
                                            Usuń
                                        </button>
                                        <button
                                            onClick={() => toggleOrderStatus(order)}
                                            className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded transition ease-in-out duration-150 focus:outline-none focus:ring-2 focus:ring-blue-200 focus:ring-opacity-50 w-full md:w-auto'
                                        >
                                            Zmień Status
                                        </button>
                                    </div>
                                </div>
                            ))
                        ) : (
                            <p>Brak zamówień do wyświetlenia.</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );

};

export default AdminPanel;
