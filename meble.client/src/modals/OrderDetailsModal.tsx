import React, { useState, useEffect } from 'react';
import { OrderDetails } from '.././pages/Order';


interface FurniturePhoto {
    photoId: number;
    photoUrl: string;
    photoDescription?: string;
}

interface OrderDetailsModalProps {
    orderDetails: OrderDetails;
    onClose: () => void;
    accessToken: string;
    fetchWithAuth: (...args: any[]) => Promise<any>;
}

const OrderDetailsModal: React.FC<OrderDetailsModalProps> = ({ orderDetails, onClose, accessToken, fetchWithAuth }) => {
    const [furniturePhotos, setFurniturePhotos] = useState<FurniturePhoto[][]>([]);

    useEffect(() => {
        const fetchPhotosForFurniture = async () => {
            const photos = await Promise.all(orderDetails.furnitures.map(async (furniture) => {
                const response = await fetchWithAuth(`https://localhost:7197/photoFurniture/getPhoto/${furniture.furnitureId}`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                });
                if (response.ok) {
                    return response.json();
                }
                return [];
            }));
            setFurniturePhotos(photos);
        };

        fetchPhotosForFurniture();
    }, [orderDetails, accessToken, fetchWithAuth]);

    return (
        <div className="modal flex flex-col items-center glass2 justify-start p-8">
            <div className=' flex flex-col items-center bg-white rounded-2xl sticky right-auto top-24 md:right-1/4'>
                <button className='min-height-full my-10' onClick={onClose}>Zamknij</button>
                <h2>Szczegóły zamówienia #{orderDetails.orderId}</h2>
                <ul className='flex flex-col'>
                    {orderDetails.furnitures.map((furniture, index) => (
                        <li className='flex gap-10 flex-col bg-white p-4 rounded my-2' key={furniture.furnitureId}>
                            {furniture.furnitureName} - {furniture.furniturePrice.toFixed(2)} zł - {orderDetails.quantityOrdered![index]} sztuk
                            <div className='flex flex-wrap justify-center'>

                                {furniturePhotos[index]?.map(photo => (
                                    <img className='border max-w-[150px]' key={photo.photoId} src={photo.photoUrl} alt={photo.photoDescription || 'Furniture'} />
                                ))}
                            </div>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default OrderDetailsModal;