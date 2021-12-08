import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

SignUpSuccessModal.propTypes = {
	isShowModal: PropTypes.bool.isRequired,
	hideModal: PropTypes.func.isRequired
};

function SignUpSuccessModal(props) {
	return (
		props.isShowModal && (
			<div
				className='modal'
				tabIndex='-1'
				aria-labelledby='signupSuccessModalHeader'>
				<div className='modal-dialog modal-dialog-centered'>
					<div className='modal-content border border-info'>
						<ModalHeader hideModal={props.hideModal} />
						<div className='modal-body'>
							<p>
								Your account has been successfully created. Now you can log in.
							</p>
						</div>
						<ModalFooter hideModal={props.hideModal} />
					</div>
				</div>
			</div>
		)
	);
}

const ModalHeader = (hideModal) => {
	return (
		<div className='modal-header'>
			<h5 className='modal-title' id='signupSuccessModalHeader'>
				Thanks for signing up
			</h5>
			<button
				type='button'
				className='btn-close'
				onClick={hideModal}
				aria-label='Close'>
				<span aria-hidden='true'>&times;</span>
			</button>
		</div>
	);
};

const ModalFooter = (hideModal) => {
	return (
		<div className='modal-footer'>
			<button type='button' className='btn btn-secondary' onClick={hideModal}>
				Close
			</button>
			<button className='button default'>
				<Link className='sign-up' to={'/login'}>
					Log in
				</Link>
			</button>
		</div>
	);
};

export default SignUpSuccessModal;
