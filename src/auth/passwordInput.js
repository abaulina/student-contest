import React, { forwardRef } from 'react';
import cn from 'classnames';
import PropTypes from 'prop-types';
import hidePwdImg from '../assets/hide-password.svg';
import showPwdImg from '../assets/show-password.svg';

const PasswordInput = forwardRef(
	(
		{
			error,
			isPasswordShown,
			capsLockWarning,
			onEnterPress,
			onPasswordInput,
			togglePassword,
			isSignUp
		},
		passwordInputRef
	) => {
		const inputClassName = cn({
			'form-control': true,
			'is-invalid': !!error
		});
		const divClassName = cn({
			'form-floating d-flex': true,
			'sign-up': isSignUp,
			'log-in': !isSignUp
		});

		return (
			<div className={divClassName}>
				<input
					ref={passwordInputRef}
					type={isPasswordShown ? 'text' : 'password'}
					className={inputClassName}
					id='floatingPassword'
					placeholder='Password'
					onKeyUp={capsLockWarning}
					onKeyPress={onEnterPress}
					onChange={onPasswordInput}
				/>
				<label htmlFor='floatingPassword'>Password</label>
				<ShowHidePasswordImage
					isPasswordShown={isPasswordShown}
					togglePassword={togglePassword}
				/>
				<div className='invalid-feedback'>{error}</div>
			</div>
		);
	}
);

PasswordInput.displayName = 'PasswordInput';

PasswordInput.propTypes = {
	error: PropTypes.any,
	onEnterPress: PropTypes.func,
	isPasswordShown: PropTypes.bool,
	capsLockWarning: PropTypes.func,
	onPasswordInput: PropTypes.func,
	togglePassword: PropTypes.func,
	isSignUp: PropTypes.bool
};

const ShowHidePasswordImage = ({ isPasswordShown, togglePassword }) => {
	return (
		<img
			htmlFor='floatingPassword'
			className='log-in'
			data-testid='pswdImg'
			alt='show or hide password'
			height='24'
			width='24'
			title={isPasswordShown ? 'Hide password' : 'Show password'}
			src={isPasswordShown ? hidePwdImg : showPwdImg}
			onClick={togglePassword}
		/>
	);
};

ShowHidePasswordImage.propTypes = {
	isPasswordShown: PropTypes.bool,
	togglePassword: PropTypes.func
};

export default PasswordInput;
