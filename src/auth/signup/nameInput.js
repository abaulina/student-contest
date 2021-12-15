import React, { forwardRef } from 'react';
import cn from 'classnames';
import PropTypes from 'prop-types';

const NameInput = forwardRef(
	({ name, error, onEnterPress, onInput }, inputRef) => {
		const inputClassName = cn({
			'form-control': true,
			'is-invalid': !!error
		});

		const inputId = 'floating' + name.replace(/\s/g, '');
		return (
			<div className='form-floating'>
				<input
					type='text'
					ref={inputRef}
					className={inputClassName}
					id={inputId}
					onChange={onInput}
					onKeyPress={onEnterPress}
					placeholder={name}
				/>
				<label htmlFor={inputId}>{name}</label>
				<div className='invalid-feedback'>{error}</div>
			</div>
		);
	}
);

NameInput.displayName = 'NameInput';

NameInput.propTypes = {
	name: PropTypes.string.isRequired,
	error: PropTypes.any,
	onEnterPress: PropTypes.func.isRequired,
	onInput: PropTypes.func.isRequired
};

export default NameInput;
