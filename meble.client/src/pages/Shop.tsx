import { useState, useContext, useEffect } from 'react';
import fetchWithAuth from '../components/utils/fetchWithAuth';
import { AuthContext } from '../services/AuthContext';
import { useDispatch } from 'react-redux';
import { addToCart } from '../components/actions/CartAction';
import DeleteFurnitureModal from '../modals/DeleteFurnitureModal';
import UpdateFurnitureModal from '../modals/UpdateFurnitureModal';
import AddFurnitureModal from '../modals/AddFurnitureModal';
import { Slide } from 'react-slideshow-image';
import 'react-slideshow-image/dist/styles.css';

export interface Furniture {
    furnitureId: number;
    furnitureName: string;
    furniturePrice: number;
    furnitureDateOfAddition?: string;
    furnitureDescription?: string;
    furnitureDateOfUpdate?: string;
    isAvailable: boolean;
    quantity: number;
    photos?: FurniturePhoto[];
    categories?: FurnitureCategory[];
}

export interface FurniturePhoto {
    photoId: number;
    photoFurnitureId: number;
    photoUrl: string;
    photoDescription: string;
    photoDateOfUpdate?: string;
}

export interface FurnitureCategory {
    categoryId: number;
    categoryName: string;
    categoryFurnitureId: number;
    categoryDateOfUpdate?: string;
}

const Shop = () => {
    const { accessToken, refreshAccessToken, logout, roles } = useContext(AuthContext);
    const [furnitures, setFurnitures] = useState<Furniture[]>([]);
    const [error, setError] = useState<string | null>(null);
    const dispatch = useDispatch();
    const isAdmin = roles.includes('Admin');
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selectedFurnitureId, setSelectedFurnitureId] = useState<number | null>(null);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [editingFurniture, setEditingFurniture] = useState<Furniture | null>(null);
    const [showAddModal, setShowAddModal] = useState(false);
    const [photos, setPhotos] = useState<FurniturePhoto[]>([]);
    const [categories, setCategories] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('');
    

    useEffect(() => {
        const fetchFurnituresAndDetails = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/furnitures/getAllFurnitures', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const data: Furniture[] = await response.json();
                    const furnituresWithDetails = await Promise.all(data.map(async (furniture) => {
                        const photosResponse = await fetchWithAuth(`https://localhost:7197/photoFurniture/getPhoto/${furniture.furnitureId}`, {
                            method: 'GET',
                            headers: { 'Authorization': `Bearer ${accessToken}` },
                        }, refreshAccessToken, logout);

                        const categoriesResponse = await fetchWithAuth(`https://localhost:7197/furnitureCategories/getCategory/${furniture.furnitureId}`, {
                            method: 'GET',
                            headers: { 'Authorization': `Bearer ${accessToken}` },
                        }, refreshAccessToken, logout);

                        const photos = photosResponse.ok ? await photosResponse.json() : [];
                        const categories = categoriesResponse.ok ? await categoriesResponse.json() : [];

                        return { ...furniture, photos, categories };
                    }));

                    setFurnitures(furnituresWithDetails);
                } else {
                    setError('Błąd podczas pobierania listy mebli');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas ładowania listy mebli');
                logout();
            }
        };

        fetchFurnituresAndDetails();
    }, [accessToken, refreshAccessToken, logout]);


    const handleDeleteClick = (furnitureId: number) => {
        setSelectedFurnitureId(furnitureId);
        setShowDeleteModal(true);
    };

    const handleConfirmDelete = async () => {
        if (selectedFurnitureId !== null) {
            await deleteFurniturePhotos(selectedFurnitureId);

            const response = await fetchWithAuth(`https://localhost:7197/furnitures/deleteFurniture/${selectedFurnitureId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                },
            }, refreshAccessToken, logout);

            if (response.ok) {
                setFurnitures(furnitures.filter(furniture => furniture.furnitureId !== selectedFurnitureId));
                setShowDeleteModal(false);
            } else {
                console.error("Błąd podczas usuwania mebla");
            }
        }
    };

    const deleteFurniturePhotos = async (furnitureId: number) => {
        try {
            await fetchWithAuth(`https://localhost:7197/photoFurniture/deletePhotosOfFurniture/${furnitureId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                },
            }, refreshAccessToken, logout);
        } catch (error) {
            console.error("Błąd podczas usuwania zdjęć mebla", error);
        }
    };

    const handleDeletePhoto = async (photoId: number) => {
        try {
            const response = await fetchWithAuth(`https://localhost:7197/photoFurniture/deletePhoto/${photoId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                },
            }, refreshAccessToken, logout);

            if (response.ok) {
                setPhotos(photos.filter((photo: { photoId: number; }) => photo.photoId !== photoId));
            } else {
                const errorText = await response.text();
                console.error("Błąd podczas usuwania zdjęcia: ", errorText);
                alert(`Wystąpił błąd: ${errorText}`);
                logout();
            }
        } catch (error) {
            console.error("Wystąpił błąd podczas usuwania zdjęcia: ", error);
            alert(`Wystąpił błąd: ${error instanceof Error ? error.message : "Nieznany błąd"}`);
            logout();
        }
    };

    const handleUpdateClick = (furniture: Furniture) => {
        setEditingFurniture(furniture);
        setPhotos(furniture.photos || []);
        setShowUpdateModal(true);
    };

    const handleSaveChanges = async (updatedFurniture: Furniture) => {
        if (!updatedFurniture.furnitureId) return;

        const updatedData = {
            ...updatedFurniture,
            furniturePrice: typeof updatedFurniture.furniturePrice === 'number'
                ? updatedFurniture.furniturePrice
                : parseFloat(updatedFurniture.furniturePrice)
        };

        try {
            const response = await fetchWithAuth(`https://localhost:7197/furnitures/updateFurniture/${updatedFurniture.furnitureId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedData)
            }, refreshAccessToken, logout);

            if (response.ok) {
                setFurnitures(prevFurnitures => {
                    return prevFurnitures.map(f => {
                        if (f.furnitureId === updatedFurniture.furnitureId) {
                            return { ...f, ...updatedFurniture };
                        }
                        return f;
                    });
                });
                setShowUpdateModal(false);
            } else {
                const errorText = await response.text();
                console.error("Błąd podczas aktualizacji mebla: ", errorText);
                alert(`Wystąpił błąd: ${errorText}`);
                logout();
            }
        } catch (error) {
            console.error("Wystąpił błąd podczas aktualizacji mebla: ", error);
            alert(`Wystąpił błąd: ${error instanceof Error ? error.message : "Nieznany błąd"}`);
            logout();
        }
    };

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await fetchWithAuth('https://localhost:7197/furnitureCategories/categories', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                    },
                }, refreshAccessToken, logout);

                if (response.ok) {
                    const data = await response.json();
                    setCategories(data);
                } else {
                    setError('Nie udało się pobrać kategorii mebli.');
                }
            } catch (error) {
                setError('Wystąpił błąd podczas pobierania kategorii mebli.');
                logout();
            }
        };

        fetchCategories();
    }, [accessToken, refreshAccessToken, logout]);

    const fetchFurnitures = async () => {
        try {
            const searchResponse = await fetchWithAuth(`https://localhost:7197/furnitures/search?name=${searchTerm}&category=${selectedCategory}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                },
            }, refreshAccessToken, logout);

            if (searchResponse.ok) {
                const searchData = await searchResponse.json();
                const furnituresWithDetails = await Promise.all(searchData.map(async (furniture: { furnitureId: any; }) => {
                    const photosResponse = await fetchWithAuth(`https://localhost:7197/photoFurniture/getPhoto/${furniture.furnitureId}`, {
                        method: 'GET',
                        headers: { 'Authorization': `Bearer ${accessToken}` },
                    }, refreshAccessToken, logout);

                    const categoriesResponse = await fetchWithAuth(`https://localhost:7197/furnitureCategories/getCategory/${furniture.furnitureId}`, {
                        method: 'GET',
                        headers: { 'Authorization': `Bearer ${accessToken}` },
                    }, refreshAccessToken, logout);

                    const photos = photosResponse.ok ? await photosResponse.json() : [];
                    const categories = categoriesResponse.ok ? await categoriesResponse.json() : [];

                    return { ...furniture, photos, categories };
                }));

                setFurnitures(furnituresWithDetails);
            } else {
                setError('Błąd podczas wyszukiwania mebli');
            }
        } catch (error) {
            setError('Wystąpił błąd podczas wyszukiwania mebli');
            logout();
        }
    };

    const handleSearch = () => {
        fetchFurnitures();
    };

    interface ArrowProps {
        onClick?: () => void;
    }

    const CustomPrevArrow: React.FC<ArrowProps> = ({ onClick }) => (
        <div style={{ position: 'absolute', bottom: '70px', left: '50px', padding: "4px", cursor: 'pointer', margin: '0 10px', zIndex: "999" }} onClick={onClick}>
            <span>&lt;</span>
        </div>
    );

    const CustomNextArrow: React.FC<ArrowProps> = ({ onClick }) => (
        <div style={{ position: 'absolute', bottom: '70px', right: '50px', padding: "4px", cursor: 'pointer', margin: '0 10px', zIndex: "999" }} onClick={onClick}>
            <span>&gt;</span>
        </div>
    );
    
    return (
        <div className='containerCustom pb-6'>
            <div className='flex flex-col items-center gap-10 mx-auto px-4 max-w-2xl'>
                {error && <div className="error-message text-white text-center">Błąd: {error}</div>}

                <input
                    type="text"
                    placeholder="Szukaj mebli..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="inputCustom w-full p-2 rounded border btn border-gray-300"
                    maxLength={30} 
                />
                <select
                    value={selectedCategory}
                    onChange={(e) => setSelectedCategory(e.target.value)}
                    className="selectCustom w-full p-2 rounded my-2 border border-gray-300"
                >
                    <option value="">Wszystkie kategorie</option>
                    {categories.map((category) => (
                        <option key={category} value={category}>{category}</option>
                    ))}
                </select>
                <button onClick={handleSearch} className="btn btn-primary w-full text-white btn rounded py-2 px-4 my-2">Szukaj</button>

                {isAdmin && (
                    <div className='w-full flex flex-col items-center'>
                        <button className='btn my-4' onClick={() => setShowAddModal(true)}>Dodaj mebel</button>
                        {showAddModal && (
                            <AddFurnitureModal
                                onClose={() => setShowAddModal(false)}
                            />
                        )}
                    </div>
                )}
            </div>
            <div className="flex flex-wrap text-wrap gap-10 mt-6 justify-center">
                {furnitures.map((furniture) => (
                    <div className="rounded transform grow transition-transform duration-500 hover:scale-105 border glass px-4 max-w-[300px] py-4 flex flex-col justify-center items-center gap-2 text-white" key={furniture.furnitureId}>
                <div className='w-full text-lg overflow-hidden'>
                            <Slide prevArrow={<CustomPrevArrow />} nextArrow={<CustomNextArrow />}>
                                {furniture.photos && furniture.photos.map(photo => (
                                    <div className='each-slide' key={photo.photoId}>
                                        <div className='flex flex-col justify-center items-center'>
                                            <img className='w-[429px] rounded' src={photo.photoUrl} alt={photo.photoDescription} />
                                            {isAdmin && <button className='my-9 text-sm' onClick={() => handleDeletePhoto(photo.photoId)}>Usuń zdjęcie</button>}
                                        </div>
                                    </div>
                                ))}
                            </Slide>
                </div>
                        <h2 className='flex-1 flex justify-center flex-col items-center font-bold text-center mb-4 p-2 gap-6'>
                            {furniture.furnitureName} - &nbsp;
                            <div className='flex flex-1 justify-center items-center flex-col'>
                                <strong className='text-lg'>Cena:</strong>
                                {typeof furniture.furniturePrice === 'number' ?
                                    furniture.furniturePrice.toFixed(2) :
                                    parseFloat(furniture.furniturePrice).toFixed(2)}
                                zł
                            </div>

                        </h2>
                        <p className='bold flex flex-col justify-center items-center gap-2'><strong className='text-lg'>Opis: </strong>{furniture.furnitureDescription}</p>
                        {furniture.furnitureDateOfAddition && <p className='bold flex flex-col justify-center items-center gap-2'><strong className='text-lg'>Data dodania:</strong> {new Date(furniture.furnitureDateOfAddition).toLocaleDateString()}</p>}
                        {furniture.furnitureDateOfUpdate && <p className='bold flex flex-col justify-center items-center gap-2'><strong className='text-lg'>Data aktualizacji:</strong> {new Date(furniture.furnitureDateOfUpdate).toLocaleDateString()}</p>}
                        <p className='bold flex flex-col justify-center items-center gap-2'><strong className='text-lg'>Dostępność:</strong> {furniture.isAvailable ? 'Dostępny' : 'Niedostępny'}</p>
                        <p className='bold flex flex-col justify-center items-center gap-2'><strong className='text-lg'>Ilość w magazynie:</strong> {furniture.quantity}</p>
                        <div>
                            {furniture.categories && furniture.categories.map(category => (
                                <span key={category.categoryId} className="furniture-category">{category.categoryName}</span>
                            ))}
                        </div>
                        <button className='transition-transform duration-500 hover:scale-105 px-4 my-2 py-2 flex justify-center items-center rounded-xl  brown' onClick={() => dispatch(addToCart({
                            id: furniture.furnitureId,
                            name: furniture.furnitureName,
                            price: furniture.furniturePrice,
                            description: furniture.furnitureDescription,
                            isAvailable: furniture.isAvailable,
                            quantity: 1,
                            maxQuantity: furniture.quantity,
                            photos: furniture.photos?.map(p => ({ url: p.photoUrl, description: p.photoDescription })),
                            categories: furniture.categories?.map(c => ({ name: c.categoryName }))
                        }))}>Dodaj do koszyka</button>

                        {isAdmin && (
                            <>
                                <button className='button' onClick={() => handleUpdateClick(furniture)}>Aktualizuj</button>
                                <button className='button' onClick={() => handleDeleteClick(furniture.furnitureId)}>Usuń</button>
                            </>
                        )}

                    </div>
                ))}

                {showDeleteModal && selectedFurnitureId !== null && (
                    <DeleteFurnitureModal
                        furnitureId={selectedFurnitureId}
                        onConfirm={handleConfirmDelete}
                        onCancel={() => setShowDeleteModal(false)}
                    />
                )}

                {showUpdateModal && editingFurniture && (
                    <UpdateFurnitureModal
                        furniture={editingFurniture}
                        onSave={handleSaveChanges}
                        onCancel={() => setShowUpdateModal(false)}
                    />
                )}

            </div>
        </div>
    )
};

export default Shop;
