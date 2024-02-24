import React from 'react';
import styled from 'styled-components';

const InputContainer = styled.div`
  margin-bottom: 15px;
`;

const InputElement = styled.input`
  padding: 10px;
  width: 100%;
  border: 1px solid #ccc;
  border-radius: 5px;
  transition: border-color 0.3s;

  &:focus {
    border-color: blue;
  }
`;


interface Props {
    type: string;
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    placeholder: string;
    maxLength?: number;
    pattern?: string;
}


const StyledInput: React.FC<Props> = ({ type, value, onChange, placeholder, maxLength, pattern }) => {
    return (
        <InputContainer>
            <InputElement
                type={type}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
                maxLength={maxLength}
                pattern={pattern}
            />
        </InputContainer>
    );
};

export default StyledInput;
