import React from 'react';
import styled from 'styled-components';

const Button = styled.button``;


interface Props {
    isLoading?: boolean;
    onClick?: () => void;
    children: React.ReactNode;
}

const LoginButton: React.FC<Props> = ({ isLoading, onClick, children }) => {
    return <Button onClick={onClick} disabled={isLoading}>{isLoading ? 'Logging in...' : children}
    </Button>;
};

export default LoginButton;
