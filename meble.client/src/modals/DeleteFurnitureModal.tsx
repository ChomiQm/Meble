import React from 'react';

interface DeleteFurnitureModalProps {
    furnitureId: number;
    onConfirm: () => void;
    onCancel: () => void;
}

const DeleteFurnitureModal: React.FC<DeleteFurnitureModalProps> = ({ furnitureId, onConfirm, onCancel }) => {
    return (
        <div className="modal h-full rounded-2xl glass2 p-8 justify-center items-start flex">
                <div className='flex justify-center flex-col items-center sticky top-[35%]  right-auto    white-bg'>
                <h2 className='text-black'>Potwierdź usunięcie mebla</h2>
                <p className='text-black'>Czy na pewno chcesz usunąć mebel o ID: {furnitureId}?</p>
                <div className='flex gap-2 pt-4'>
                    <button className='transition-transform duration-500 hover:scale-105 px-3 my-1 p-1 flex justify-center items-center rounded-xl text-white brown' onClick={onConfirm}>Usuń</button>
                    <button className='transition-transform duration-500 hover:scale-105 px-3 my-1 p-1 flex justify-center items-center rounded-xl text-white brown' onClick={onCancel}>Anuluj</button>
                </div>
            </div>
        </div>
    );
};

export default DeleteFurnitureModal;
