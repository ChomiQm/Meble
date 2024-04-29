import React, { useState, useContext, useCallback } from 'react';
import { AuthContext } from '../services/AuthContext';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { useDropzone } from 'react-dropzone';

interface AddFurnitureModalProps {
    onClose: () => void;
}

interface PhotoWithDescription {
    file: File;
    description: string;
}


const PhotoList: React.FC<{ photos: PhotoWithDescription[], onPhotoDescriptionChange: (index: number, description: string) => void }> = ({ photos, onPhotoDescriptionChange }) => {
    return (
        <div>
            {photos.map((photo, index) => (
                <div key={index}>
                    <div>{photo.file.name}</div>
                    <input
                        type="text"
                        value={photo.description}
                        onChange={(e) => onPhotoDescriptionChange(index, e.target.value)}
                        placeholder="Opis zdjęcia"
                    />
                </div>
            ))}
        </div>
    );
};

const AddFurnitureModal: React.FC<AddFurnitureModalProps> = ({ onClose }) => {
    const { accessToken, refreshAccessToken, logout } = useContext(AuthContext);
    const [newFurniture, setNewFurniture] = useState({
        furnitureName: '',
        furniturePrice: 0,
        isAvailable: true,
        quantity: 0,
        furnitureDescription: ''
    });
    const [photos, setPhotos] = useState<PhotoWithDescription[]>([]);
    const [categoryName, setCategoryName] = useState('');

    const onDrop = useCallback((acceptedFiles: File[]) => {
        const newPhotos = acceptedFiles.map(file => ({ file, description: '' }));
        setPhotos(prevPhotos => [...prevPhotos, ...newPhotos]);
    }, []);

    const handlePhotoDescriptionChange = (index: number, description: string) => {
        setPhotos(photos.map((photo, idx) => {
            if (idx === index) {
                return { ...photo, description };
            }
            return photo;
        }));
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        setNewFurniture({
            ...newFurniture,
            [e.target.name]: e.target.value
        });
    };

    const handleCategoryChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setCategoryName(e.target.value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const furnitureResponse = await fetchWithAuth(`https://mebloartbackend.azurewebsites.net/furnitures/addFurniture`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${accessToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newFurniture)
        }, refreshAccessToken, logout);

        if (furnitureResponse.ok) {
            const addedFurniture = await furnitureResponse.json();

            if (categoryName) {
                await fetchWithAuth(`https://mebloartbackend.azurewebsites.net/furnitureCategories/addCategory`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ categoryName, categoryFurnitureId: addedFurniture.furnitureId })
                }, refreshAccessToken, logout);
            }

            for (const photo of photos) {
                const formData = new FormData();
                formData.append('file', photo.file);
                formData.append('photoDescription', photo.description);

                try {
                    const photoResponse = await fetchWithAuth(`https://mebloartbackend.azurewebsites.net/photoFurniture/addPhoto/${addedFurniture.furnitureId}`, {
                        method: 'POST',
                        headers: {
                            'Authorization': `Bearer ${accessToken}`,
                        },
                        body: formData
                    }, refreshAccessToken, logout);

                    if (!photoResponse.ok) {
                        const errorData = await photoResponse.json();
                        console.error("Błąd podczas dodawania zdjęcia mebla:", errorData);
                    }
                } catch (error) {
                    console.error("Wystąpił błąd podczas komunikacji z serwerem:", error);
                }
            }
        }

        onClose();
    };

    const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

    return (
        <div className='modal z-[100]'>
            <div className="flex h-full items-start justify-center glass2 p-4">
                <form className='flex flex-col  items-center white-bg gap-2 p-4 rounded-2xl sticky right-auto top-24  text-black' onSubmit={handleSubmit}>
                    <button className='transition-transform duration-500 hover:scale-105 px-3 my-2 p-1 text-white flex justify-center items-center rounded-xl brown max-w-[160px]' onClick={onClose}>Zamknij</button>
                    <label>Nazwa mebla</label>
                    <input
                        className='rounded p-1 border border-gray-500'
                        type="text"
                        name="furnitureName"
                        value={newFurniture.furnitureName}
                        onChange={handleChange}
                        placeholder="Nazwa mebla"
                    />
                    <label>Cena</label>
                    <input
                        className='rounded  text-gray-500 p-1 border border-gray-500'
                        type="number"
                        name="furniturePrice"
                        value={newFurniture.furniturePrice}
                        onChange={handleChange}
                        placeholder="Cena"
                    />
                    <label>Opis mebla</label>
                    <textarea
                        className='rounded p-1 border border-gray-500'
                        name="furnitureDescription"
                        value={newFurniture.furnitureDescription}
                        onChange={handleChange}
                        placeholder="Opis mebla"
                    />
                    <label>Ilość w magazynie</label>
                    <input
                        className='rounded text-gray-500 p-1 border border-gray-500'
                        type="number"
                        name="quantity"
                        value={newFurniture.quantity}
                        onChange={handleChange}
                        placeholder="Ilość w magazynie"
                    />
                    <label className='flex gap -2 '>
                        Dostępność: <br />
                        <input
                            className='rounded mx-3 p-2'
                            type="checkbox"
                            name="isAvailable"
                            checked={newFurniture.isAvailable}
                            onChange={e => setNewFurniture({ ...newFurniture, isAvailable: e.target.checked })}
                        />
                    </label>

                    <div {...getRootProps()} className="dropzone">
                        <input {...getInputProps()} />
                        {isDragActive ? <p>Upuść plik tutaj...</p> : <p>Przeciągnij i upuść zdjęcie mebla tutaj, lub kliknij, aby wybrać plik</p>}
                    </div>

                    <PhotoList photos={photos} onPhotoDescriptionChange={handlePhotoDescriptionChange} />
                    <label>Nazwa kategorii</label>
                    <input
                        className='rounded text-gray-500 p-1 border border-gray-500'
                        type="text"
                        name="categoryName"
                        value={categoryName}
                        onChange={handleCategoryChange}
                        placeholder="Nazwa kategorii"
                    />

                    <button className='transition-transform duration-500 hover:scale-105 px-3 my-2 p-1 text-white flex justify-center items-center rounded-xl brown max-w-[160px]' type="submit">Dodaj mebel</button>
                </form>
            </div>
        </div>
    );
};

export default AddFurnitureModal;