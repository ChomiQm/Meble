// Orders.tsx
import { useState, useContext, useEffect } from 'react';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';
import OrderDetailsModal from '.././modals/OrderDetailsModal';
import { translateStatus } from '../components/utils/translateStatus';

export interface Furniture {
    furnitureId: number;
    furnitureName: string;
    furniturePrice: number;
}

export interface OrderDetails {
    orderId: number;
    orderDateOfOrder?: string;
    totalOrderValue: number;
    totalItemsOrdered: number;
    furnitures: Furniture[];
    orderStatus: string;
    quantityOrdered?: number[];
}

const Orders = () => {
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [orders, setOrders] = useState<OrderDetails[]>([]);
    const [selectedOrder, setSelectedOrder] = useState<OrderDetails | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchUserOrders = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/order/userOrders', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const data: OrderDetails[] = await response.json();
                    setOrders(data);
                } else {
                    setError('Błąd podczas pobierania zamówień użytkownika');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas ładowania zamówień użytkownika');
                logout();
            }
        };

        fetchUserOrders();
    }, [accessToken, refreshAccessToken, logout]);

    const handleOrderClick = (order: OrderDetails) => {
        setSelectedOrder(order); 
    };

    const handleCloseModal = () => {
        setSelectedOrder(null);
    };

    if (!accessToken) {
        return <div>Musisz być zalogowany, aby zobaczyć zamówienia.</div>;
    }

    return (
        <div className='containerCustom min-h-screen w-full flex justify-center items-start'>
            <div className='flex flex-col justify-center items-center w-full gap-10 text-black containerCustom'>
                {error && <div>Błąd: {error}</div>}
                {orders.length > 0 ? (
                    <div className='w-full h-full'>
                        <div className='grid grid-cols-2 gap-5 cursor-pointer w-full rounded-2xl bg-white/30 backdrop-blur-sm rounded-lg p-4 shadow-lg'>
                            {orders.map(order => (
                                <div className='p-4 flex flex-col justify-start bg-white rounded-2xl' key={order.orderId} onClick={() => handleOrderClick(order)}>
                                    <div className="flex flex-col w-full md:flex-row md:place-content-between">
                                        <h2 className='text-xl'>Zamówienie #{order.orderId}</h2>
                                        <p>Data zamówienia: {order.orderDateOfOrder ?? 'Brak daty'}</p>
                                    </div>

                                    <p>Całkowita wartość zamówienia: {order.totalOrderValue.toFixed(2)} zł</p>
                                    <p>Ilość mebli: {order.totalItemsOrdered}</p>
                                    <p>Status zamówienia: {translateStatus(order.orderStatus)}</p>
                                </div>
                            ))}
                        </div>
                        {selectedOrder && (
                            <OrderDetailsModal
                                orderDetails={selectedOrder}
                                onClose={handleCloseModal}
                                fetchWithAuth={fetchWithAuth}
                                accessToken={accessToken}
                            />
                        )}
                    </div>
                ) : (
                    <p>Ładowanie danych zamówień...</p>
                )}
            </div>
        </div>
    );
};

export default Orders;