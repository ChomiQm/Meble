// UpdateFurnitureModal.tsx
import React, { useContext, useState } from 'react';
import { Furniture } from '../pages/Shop';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';

interface UpdateFurnitureModalProps {
    furniture: Furniture;
    onSave: (updatedFurniture: Furniture) => void;
    onCancel: () => void;
}

const UpdateFurnitureModal: React.FC<UpdateFurnitureModalProps> = ({ furniture, onSave, onCancel }) => {
    const [updatedFurniture, setUpdatedFurniture] = useState<Furniture>({ ...furniture });
    const [newPhotos, setNewPhotos] = useState<{ file: File, description: string }[]>([]);
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);

    const handleFurnitureChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        setUpdatedFurniture({ ...updatedFurniture, [e.target.name]: e.target.value });
    };

    const handlePhotoDescriptionChange = (photoId: number, description: string) => {
        const updatedPhotos = updatedFurniture.photos?.map(photo =>
            photo.photoId === photoId ? { ...photo, photoDescription: description } : photo
        );
        setUpdatedFurniture({ ...updatedFurniture, photos: updatedPhotos });
    };

    const handleNewPhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            const filesArray = Array.from(e.target.files); // Zamiana FileList na Array
            const newPhotosArray = filesArray.map(file => ({ file, description: '' }));
            setNewPhotos(prev => [...prev, ...newPhotosArray]);
        }
    };


    const handleNewPhotoDescriptionChange = (index: number, description: string) => {
        setNewPhotos(newPhotos.map((photo, idx) => idx === index ? { ...photo, description } : photo));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        // Tutaj możesz umieścić logikę zapisywania zmian w meblu
        onSave(updatedFurniture);

        // Dodawanie nowych zdjęć
        for (const photo of newPhotos) {
            const formData = new FormData();
            formData.append('file', photo.file);
            formData.append('photoDescription', photo.description);

            try {
                const photoResponse = await fetchWithAuth(`https://localhost:7197/photoFurniture/addPhoto/${updatedFurniture.furnitureId}`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                    body: formData
                }, refreshAccessToken, logout);

                if (photoResponse.ok) {
                    console.log('Zdjęcie dodane pomyślnie.');
                } else {
                    console.error('Błąd podczas dodawania zdjęcia.');
                }
            } catch (error) {
                console.error('Wystąpił błąd:', error);
            }
        }
    };

    return (
        <div className="modal border glass2 flex justify-center items-start ">
            <form className='flex flex-col  items-center white-bg gap-2 p-4 rounded-2xl sticky right-auto top-24 md:top-24    text-black' onSubmit={handleSubmit}>
                <label>Nazwa mebla</label>
                <input className='rounded p-1 border border-gray-500' type="text" name="furnitureName" value={updatedFurniture.furnitureName} onChange={handleFurnitureChange} />
                <label>Cena</label>
                <input className='rounded p-1 border border-gray-500' type="number" name="furniturePrice" value={updatedFurniture.furniturePrice} onChange={handleFurnitureChange} />
                <label>Opis mebla</label>
                <textarea className='rounded p-1 border border-gray-500' name="furnitureDescription" value={updatedFurniture.furnitureDescription || ''} onChange={handleFurnitureChange} />
                <label>Ilość w magazyniea</label>
                <input className='rounded p-1 border border-gray-500' type="number" name="quantity" value={updatedFurniture.quantity} onChange={handleFurnitureChange} />

                {updatedFurniture.photos?.map(photo => (
                    <div className='flex items-center flex-col gap-2' key={photo.photoId}>
                        <img src={photo.photoUrl} alt={photo.photoDescription} style={{ maxWidth: '100px' }} />
                        <label>Opis zdjęcia</label>
                        <input className='rounded p-1 border border-gray-500' type="text" value={photo.photoDescription} onChange={(e) => handlePhotoDescriptionChange(photo.photoId, e.target.value)} />
                    </div>
                ))}

                {/* Dodawanie nowego zdjęcia */}
                <div className='flex items-center justify-center flex-col'>
                    <input className='rounded p-1 text-black' type="file" multiple onChange={handleNewPhotoChange} />
                    {newPhotos.map((photo, index) => (
                        <div key={index}>
                            <span>{photo.file.name}</span>
                            <input
                                className='rounded p-1'
                                type="text"
                                value={photo.description}
                                onChange={(e) => handleNewPhotoDescriptionChange(index, e.target.value)}
                                placeholder="Opis zdjęcia"
                            />
                        </div>
                    ))}
                </div>

                <button className='transition-transform duration-500 hover:scale-105 px-3 my-1 p-1 flex justify-center items-center rounded-xl text-white brown' type="submit">Zapisz zmiany</button>
                <button className='transition-transform duration-500 hover:scale-105 px-3 p-1 flex justify-center items-center rounded-xl text-white brown' onClick={onCancel}>Anuluj</button>
            </form>
        </div>
    );
};

export default UpdateFurnitureModal;
