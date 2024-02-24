import React from 'react';
import styled from 'styled-components';

const Button = styled.button``;

interface Props {
    isLoading: boolean;
    children: React.ReactNode;
}

const RegisterButton: React.FC<Props> = ({ isLoading, children }) => {
    return (
        <Button disabled={isLoading}>
            {isLoading ? 'Registering...' : children}
        </Button>
    );
}

export default RegisterButton;
