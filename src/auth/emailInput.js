import React from 'react';
import cn from 'classnames';
import PropTypes from 'prop-types';

export const EmailInput = ({ email, error, onEnterPress, onInput }) => {
	const inputClassName = cn({
		'form-control': true,
		'is-invalid': !!error
	});

	return (
		<div className='form-floating'>
			<input
				value={email || ''}
				type='email'
				className={inputClassName}
				id='floatingEmail'
				placeholder='name@example.com'
				onKeyPress={onEnterPress}
				onChange={onInput}
				autoFocus
			/>
			<label htmlFor='floatingEmail'>Email address</label>
		</div>
	);
};

EmailInput.propTypes = {
	email: PropTypes.string.isRequired,
	inputRef: PropTypes.any,
	error: PropTypes.any,
	onEnterPress: PropTypes.func.isRequired,
	onInput: PropTypes.func.isRequired
};
