import React from 'react';
import styled from 'styled-components';

const Button = styled.button`
  padding: 8px 16px;
  border: 2px solid white;
  border-radius: 9999px;
  background-color: transparent; 
  color: white; // Biały tekst
  cursor: pointer; // Kursor wskazujący
  transition: all 0.3s ease-in-out; 

  &:hover {
    background-color: white;
    color: black; 
  }

  &:disabled {
    border-color: grey;
    color: grey;
    cursor: not-allowed;
  }
`;

interface Props {
    isLoading?: boolean;
    onLogout: () => void;
}

const LogoutButton: React.FC<Props> = ({ isLoading, onLogout }) => {
    return (
        <Button onClick={onLogout} disabled={isLoading}>
            {isLoading ? 'Logging out...' : 'Wyloguj się'}
        </Button>
    );
};

export default LogoutButton;
