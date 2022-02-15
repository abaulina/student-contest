import React from 'react';
import cn from 'classnames';
import PropTypes from 'prop-types';

const EmailInput = ({ email, error, onEnterPress, onInput, hasFeedback }) => {
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
			{hasFeedback && <div className='invalid-feedback'>{error}</div>}
		</div>
	);
};

EmailInput.propTypes = {
	email: PropTypes.string,
	error: PropTypes.any,
	onEnterPress: PropTypes.func.isRequired,
	onInput: PropTypes.func.isRequired,
	hasFeedback: PropTypes.bool.isRequired
};

export default EmailInput;
